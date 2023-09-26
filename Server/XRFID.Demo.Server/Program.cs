using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MQTTnet.AspNetCore;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Xerum.XFramework.Common;
using Xerum.XFramework.Common.Exceptions;
using Xerum.XFramework.DefaultLogging;
using XRFID.Demo.Modules.Mqtt;
using XRFID.Demo.Modules.Mqtt.Consumers;
using XRFID.Demo.Modules.Mqtt.Publishers;
using XRFID.Demo.Pages.Hubs;
using XRFID.Demo.Server.Consumers.Frontend;
using XRFID.Demo.Server.Consumers.Mqtt;
using XRFID.Demo.Server.Database;
using XRFID.Demo.Server.Mapper;
using XRFID.Demo.Server.Repositories;
using XRFID.Demo.Server.Services;
using XRFID.Demo.Server.StateMachines.Shipment.Consumers;
using XRFID.Demo.Server.StateMachines.Shipment.Contracts;
using XRFID.Demo.Server.StateMachines.Shipment.StateMachines;
using XRFID.Demo.Server.StateMachines.Shipment.States;
using XRFID.Demo.Server.Utilities;
using XRFID.Demo.Server.Workers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = LogHelper.CreateStatic(builder.Configuration);

Log.ForContext<Program>().Information("Host starting...");

try
{
    builder.Host.UseWindowsService(ws => ws.ServiceName = "XRFID Demo Server");

    builder.Host.AddLogging(Log.Logger);

    // Add services to the container.

    builder.WebHost.ConfigureKestrel(options =>
    {
        int port = builder.Configuration.GetValue("Mqtt:MqttPort", 1883);
        if (port <= 0)
        {
            port = 1883;
            Log.ForContext<Program>().Warning("Using default MQTT port ({port})", port);
        }
        //todo: port setup using appsettings.json
        options.ListenAnyIP(port, l => l.UseMqtt());
    });

    builder.Services.AddHostedMqttServer(mqttServer => mqttServer.WithoutDefaultEndpoint());
    builder.Services.AddMqttConnectionHandler();
    builder.Services.AddConnections();

    #region Mud
    builder.Services.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = false;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 10000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    });
    #endregion

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
    builder.Services.AddTransient<LabelService>();
    builder.Services.AddTransient<PrinterService>();
    #endregion

    builder.Services.AddDbContext<XRFIDSampleContext>(optionsAction: options => options.UseSqlite("Data Source = Persist/persist.db"));

    #region RepositorySetup
    builder.Services.AddScoped<UnitOfWork>();

    builder.Services.AddScoped<ProductRepository>();
    builder.Services.AddScoped<SkuRepository>();
    builder.Services.AddScoped<LoadingUnitRepository>();
    builder.Services.AddScoped<LoadingUnitItemRepository>();
    builder.Services.AddScoped<MovementRepository>();
    builder.Services.AddScoped<MovementItemRepository>();
    builder.Services.AddScoped<ReaderRepository>();
    builder.Services.AddScoped<LabelRepository>();
    builder.Services.AddScoped<PrinterRepository>();
    #endregion

    #region SwaggerSetup
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "XRFID Demo Api", Version = "v1" });
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

    #region MQTT

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
    #endregion

    builder.Services.AddScoped<CommandUtility>();
    builder.Services.AddScoped<GpoUtility>();

    #region Mass Transit

    builder.Services.AddScoped<IZebraMqttCommandPublisher, ZebraMqttCommandPublisher>();
    builder.Services.AddScoped<IZebraMqttEventPublisher, ZebraMqttEventPublisher>();

    builder.Services.AddMassTransit(mt =>
    {
        //Modules.Mqtt.Consumers
        mt.AddConsumer<ZebraCommandConsumer>();

        //Server.Consumer.Mqtt
        mt.AddConsumer<GpioDataConsumer, GpioDataConsumerDefinition>();
        mt.AddConsumer<HeartbeatConsumer>();
        mt.AddConsumer<MresponseConsumer>();
        mt.AddConsumer<TagDataConsumer, TagDataConsumerDefinition>();

        //Server.Consumers.Frontend;
        mt.AddConsumer<UpdateUiConsumer>();

        #region State Machine
        mt.AddConsumersFromNamespaceContaining<ShipmentGpioConsumer>();
        mt.AddRequestClient<ShipmentGpiData>();
        mt.AddSagaStateMachine<ShipmentStateMachine, ShipmentState, ShipmentStateMachineDefinition>().InMemoryRepository();
        #endregion

        mt.AddDelayedMessageScheduler();

        mt.UsingInMemory((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
            cfg.UseDelayedMessageScheduler();
        });
    });
    #endregion

    #region Workers
    builder.Services.AddSingleton<CheckPageWorker>();

    //demo data
    builder.Services.AddHostedService<InitializeWorker>();
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
    app.MapHub<UiMessageHub>("/uimessagehub");
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
