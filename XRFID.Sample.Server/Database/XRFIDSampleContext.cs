using Microsoft.EntityFrameworkCore;
using XRFID.Sample.Server.Entities;

namespace XRFID.Sample.Server.Database;

public class XRFIDSampleContext : DbContext
{
    public DbSet<LoadingUnit> LoadingUnits { get; set; }
    public DbSet<LoadingUnitItem> LoadingUnitItems { get; set; }

    public DbSet<Movement> Movements { get; set; }
    public DbSet<MovementItem> MovementItems { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Reader> Readers { get; set; }

    public XRFIDSampleContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoadingUnit>().HasKey(k => k.Id);
        modelBuilder.Entity<LoadingUnitItem>().HasKey(k => k.Id);
        modelBuilder.Entity<Movement>().HasKey(k => k.Id);
        modelBuilder.Entity<MovementItem>().HasKey(k => k.Id);
        modelBuilder.Entity<Product>().HasKey(k => k.Id);
        modelBuilder.Entity<Reader>().HasKey(k => k.Id);

        base.OnModelCreating(modelBuilder);
    }
}
