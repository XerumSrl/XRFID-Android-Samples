using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
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
            for (int i = 0; i < 20; i++)
            {
                Sku s = new Sku
                {
                    Name = "sku_" + i.ToString(),
                    Code = "sku_" + i.ToString(),
                    Description = "sku_" + i.ToString() + "_Description",
                    Products = new(),
                };

                for (int j = 0; j < 6; j++)
                {
                    Product p = new Product
                    {
                        Name = "product_" + j.ToString(),
                        Code = "product_" + j.ToString(),
                        Description = "product_" + j.ToString() + "_description",
                        Epc = "product_" + j.ToString() + "_epc",
                        SerialNumber = Guid.NewGuid().ToString(),
                        CreationTime = DateTime.Now,
                    };
                    s.Products.Add(p);
                }
                ctx.Skus.Add(s);
            }
            await ctx.SaveChangesAsync();
        }

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {

    }
}
