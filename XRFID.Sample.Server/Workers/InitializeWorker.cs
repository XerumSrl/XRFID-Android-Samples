using Microsoft.EntityFrameworkCore;
using XRFID.Sample.Server.Database;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Workers;

public sealed class InitializeWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InitializeWorker> _logger;

    public InitializeWorker(IServiceProvider serviceProvider, ILogger<InitializeWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var ctx = scope.ServiceProvider.GetRequiredService<XRFIDSampleContext>();

        var pendingMigrations = await ctx.Database.GetPendingMigrationsAsync();

        foreach (var migration in pendingMigrations)
        {
            _logger.LogWarning("[StartAsync] Missing Migration: {migration}.", migration);
        }

        if (ctx.Database.GetPendingMigrations().Any())
        {
            _logger.LogWarning("[StartAsync] Applying missing migrations... Please wait.");
            try
            {
                ctx.Database.Migrate();
                _logger.LogWarning("[StartAsync] Database migrations successfully applied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[StartAsync] Unable to apply required migrations.");
            }
        }
        else
        {
            _logger.LogInformation("[StartAsync] Database: migrations are up to date.");
        }

        if (await ctx.Database.EnsureCreatedAsync())
        {
            _logger.LogWarning("[StartAsync] Database: created for the first time.");
        }
        else
        {
            _logger.LogInformation("[StartAsync] Database: exists and has tables.");
        }

        //demo data
        if (!ctx.Skus.Any())
        {
            Random rng = new Random();
            for (int i = 0; i < 16; i++)
            {
                Sku s = new Sku
                {
                    Name = "sku_" + i.ToString(),
                    Code = "sku_" + i.ToString(),
                    Description = "sku_" + i.ToString() + "_Description",
                    Products = new(),
                };

                for (int j = 0; j < 4; j++)
                {
                    Product p = new Product
                    {
                        Name = $"product_{i}_{j}",
                        Code = $"product_{i}_{j}",
                        Description = $"product_{i}_{j}_description",
                        //Epc = $"{Guid.NewGuid():N}".ToUpperInvariant(),
                        Epc = rng.Next(0, 2) == 0 ? $"{Guid.NewGuid():N}".ToUpperInvariant() : $"{Guid.NewGuid():N}".Substring(0, 24).ToUpperInvariant(),
                        SerialNumber = Guid.NewGuid().ToString(),
                        CreationTime = DateTime.Now,
                    };
                    s.Products.Add(p);
                }
                ctx.Skus.Add(s);
            }
            await ctx.SaveChangesAsync();
        }

        if (!ctx.Printers.Any())
        {
            Printer p = new Printer
            {
                Name = "default printer",
                Code = "default printer",
                Description = "default printer",
                Ip = "192.168.1.2",
                Port = 9100,
                Language = Xerum.XFramework.Common.Enums.PrinterLanguage.PGL,
                LicenseStatus = Xerum.XFramework.Common.Enums.LicenseStatus.VALID,
                Manufacturer = Xerum.XFramework.Common.Enums.PrinterManufacturers.Printronix,
            };
            ctx.Printers.Add(p);
            await ctx.SaveChangesAsync();
        }

        if (!ctx.Labels.Any())
        {
            Label l = new Label
            {
                Name = "default label",
                Code = "default label",
                Description = "default label",
                Language = Xerum.XFramework.Common.Enums.PrinterLanguage.PGL,
                Version = 1,
                IsActive = true,
                Content = @"~NORMAL
~DELETE LOGO;*ALL
~CREATE;LAB;97
SCALE;DOT;203;203
ISET;0
FONT;FACE 92250;BOLD OFF;SLANT OFF
ALPHA
POINT;243;79;19;20;""{barcode}""
STOP
BARCODE
C128C;XRD4:4:8:8:12:12:16:16;H4.92;22;31
""'{barcode}""
PDF;S
STOP
RFWTAG;96
96;H;""{EPC}""
STOP
END
~EXECUTE;LAB

~NORMAL"
            };
            ctx.Labels.Add(l);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {

    }
}
