using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Serilog;
using Xerum.XFramework.Common;
using Xerum.XFramework.DefaultLogging;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Mapper;
using XRFID.Sample.Server.Repositories;
using XRFID.Sample.Server.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = LogHelper.CreateStatic(builder.Configuration);

Log.ForContext<Program>().Information("Host starting...");

try
{
    builder.Host.UseWindowsService(ws => ws.ServiceName = "XRFID sample Server");

    builder.Host.AddLogging(Log.Logger);

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();

    builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(x => x.SuppressMapClientErrors = true)
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddSingleton<XResponseDataFactory>();

    builder.Services.AddAutoMapper(m => m.AddProfile<XRFIDSampleProfile>());

    builder.Services.AddTransient<ProductService>();
    builder.Services.AddTransient<LoadingUnitService>();
    builder.Services.AddTransient<ReaderService>();
    builder.Services.AddTransient<LoadingUnitItemService>();
    builder.Services.AddTransient<MovementService>();

    builder.Services.AddScoped<UnitOfWork>();

    builder.Services.AddDbContext<XRFIDSampleContext>(optionsAction: options => options.UseSqlite("Data Source = Persist/persist.db"));

    builder.Services.AddScoped<ProductRepository>();
    builder.Services.AddScoped<LoadingUnitRepository>();
    builder.Services.AddScoped<LoadingUnitItemRepository>();
    builder.Services.AddScoped<MovementRepository>();
    builder.Services.AddScoped<MovementItemRepository>();
    builder.Services.AddScoped<ReaderRepository>();

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

    var app = builder.Build();

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

    app.UseSwagger();
    app.UseSwaggerUI();

    if (builder.Configuration.GetValue<bool>("ForceHttps", false))
    {
        app.UseHttpsRedirection();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseStaticFiles();

    app.UseRouting();

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
