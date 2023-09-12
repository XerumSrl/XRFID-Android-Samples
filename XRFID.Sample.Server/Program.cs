using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MQTTnet.AspNetCore;
using Serilog;
using Xerum.XFramework.Common;
using Xerum.XFramework.Common.Exceptions;
using Xerum.XFramework.DefaultLogging;
using XRFID.Sample.Modules.Mqtt;
using XRFID.Sample.Modules.Mqtt.Consumers;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Mapper;
using XRFID.Sample.Server.Repositories;
using XRFID.Sample.Server.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = LogHelper.CreateStatic(builder.Configuration);

Log.ForContext<Program>().Information("Host starting...");

try
{
    builder.Host.UseWindowsService(ws => ws.ServiceName = "XRFID sample Server");

    builder.Host.AddLogging(Log.Logger);

    // Add services to the container.

    builder.WebHost.ConfigureKestrel(options =>
    {
        //todo: port setup using appsettings.json
        options.ListenAnyIP(1883, l => l.UseMqtt());
    });

    builder.Services.AddHostedMqttServer(mqttServer => mqttServer.WithoutDefaultEndpoint());
    builder.Services.AddMqttConnectionHandler();
    builder.Services.AddConnections();

    #region BlazorSetup
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    #endregion

    #region ControllerSetup
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(x => x.SuppressMapClientErrors = true)
        .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddSingleton<XResponseDataFactory>();
    #endregion

    builder.Services.AddAutoMapper(m => m.AddProfile<XRFIDSampleProfile>());

    #region ServiceSetup
    builder.Services.AddTransient<ProductService>();
    builder.Services.AddTransient<LoadingUnitService>();
    builder.Services.AddTransient<ReaderService>();
    builder.Services.AddTransient<LoadingUnitItemService>();
    builder.Services.AddTransient<MovementService>();
    #endregion

    builder.Services.AddDbContext<XRFIDSampleContext>(optionsAction: options => options.UseSqlite("Data Source = Persist/persist.db"));

    #region RepositorySetup
    builder.Services.AddScoped<UnitOfWork>();

    builder.Services.AddScoped<ProductRepository>();
    builder.Services.AddScoped<LoadingUnitRepository>();
    builder.Services.AddScoped<LoadingUnitItemRepository>();
    builder.Services.AddScoped<MovementRepository>();
    builder.Services.AddScoped<MovementItemRepository>();
    builder.Services.AddScoped<ReaderRepository>();
    #endregion

    #region SwaggerSetup
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "XRFID Sample Api", Version = "v1" });
        c.AddSecurityDefinition(
            "oauth2",
            new OpenApiSecurityScheme
            {
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        Scopes = new Dictionary<string, string>
                        {
                            ["api"] = "api scope"
                        },
                        TokenUrl = new Uri("/connect/token", UriKind.Relative),
                    },
                },
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization,
                Type = SecuritySchemeType.OAuth2
            }
        );
        c.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }

                    },
                    new[] { "api" }
                }
            });

        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        c.IgnoreObsoleteActions();
        c.IgnoreObsoleteProperties();
        c.CustomSchemaIds(type => type.FullName);
    });
    #endregion

    #region OpenIDDictSetup
    builder.Services.AddAuthorization();
    builder.Services.AddOpenIddict()
        .AddCore(options =>
        {
            options.UseEntityFrameworkCore().UseDbContext<XRFIDSampleContext>();
        })
        .AddServer(options =>
        {
            options.SetTokenEndpointUris("connect/token");
            //options.SetIntrospectionEndpointUris("/connect/introspect");
            options.AllowClientCredentialsFlow();

            //options.AddDevelopmentEncryptionCertificate();
            //options.AddDevelopmentSigningCertificate();
            options.AddEphemeralEncryptionKey()
                   .AddEphemeralSigningKey();

            // Note: to use JWT access tokens instead of the default
            // encrypted format, the following lines are required:
            //options.DisableAccessTokenEncryption();

            options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();
        })
        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });
    #endregion

    #region MQTT & MassTransit

    builder.Services.AddZebraManagedMqttClient(m =>
    {
        string? configValue = builder.Configuration.GetValue<string>("Mqtt:MqttClientId");
        if (string.IsNullOrWhiteSpace(configValue))
        {
            configValue = Guid.NewGuid().ToString();
            Log.ForContext<Program>().Information("Using new MqttClientId: ({guid})", configValue);
        }
        m.ClientId = configValue;


        configValue = builder.Configuration.GetValue<string>("Mqtt:MqttServer");
        if (string.IsNullOrWhiteSpace(configValue))
        {
            throw new NotConfiguredException(paramName: "Mqtt:MqttServer");
        }
        m.Server = configValue;


        int port = builder.Configuration.GetValue("Mqtt:MqttPort", 1883);
        if (port <= 0)
        {
            port = 1883;
            Log.ForContext<Program>().Warning("Using default MQTT port ({port})", port);
        }
        m.Port = port;

    });

    builder.Services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);

    builder.Services.AddMassTransit(mt =>
    {
        mt.AddConsumer<ZebraCommandConsumer>();

        mt.UsingInMemory();
    });


    #endregion

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        if (builder.Configuration.GetValue<bool>("ForceHttps", false))
        {
            app.UseHsts();
        }
    }

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapConnectionHandler<MqttConnectionHandler>(
            "/mqtt",
            httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                protocolList => protocolList.FirstOrDefault() ?? string.Empty);
    });

    app.UseMqttServer(server =>
    {
        /*
            * Attach event handlers etc. if required.
            */

        //server.ValidatingConnectionAsync += mqttController.ValidateConnection;
        //server.ClientConnectedAsync += mqttController.OnClientConnected;
    });

    app.UseSwagger();
    app.UseSwaggerUI();

    if (builder.Configuration.GetValue<bool>("ForceHttps", false))
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();



    app.MapControllers();
    app.MapDefaultControllerRoute();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();

    return 0;
}
catch (HostAbortedException)//EF core migration genaration causes this to be thrown, and it pollutes the logs
{
    return 1;
}
catch (Exception ex)
{
    Log.ForContext<Program>().Fatal(ex, "Host terminated unexpectedly");
    return -1;
}
finally
{
    Log.CloseAndFlush();
}
