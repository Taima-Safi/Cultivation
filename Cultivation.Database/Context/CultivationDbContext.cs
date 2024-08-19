
using Cultivation.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cultivation.Database.Context;

public class CultivationDbContext : DbContext
{
    public CultivationDbContext()
    {
    }

    public CultivationDbContext(DbContextOptions<CultivationDbContext> options) : base(options)
    {
    }
    public DbSet<LandModel> Land { get; set; }
    public DbSet<CuttingModel> Cutting { get; set; }
    public DbSet<ColorModel> Color { get; set; }
    public DbSet<CuttingLandModel> CuttingLand { get; set; }
    public DbSet<CuttingColorModel> CuttingColor { get; set; }
    public DbSet<FertilizerModel> Fertilizer { get; set; }
    public DbSet<FertilizerLandModel> FertilizerLand { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder builder) // for relations
    {
        base.OnModelCreating(builder);

        builder.Entity<LandModel>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }
    protected void BeforeSaving()
    {
        IEnumerable<EntityEntry> entityEntries = ChangeTracker.Entries();
        DateTime utcNow = DateTime.UtcNow;
        foreach (var entityEntry in entityEntries)
        {
            if (entityEntry.Entity is BaseModel entity)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Modified:
                        entity.UpdateDate = utcNow;
                        break;
                    case EntityState.Added:
                        entity.CreateDate = utcNow;
                        break;
                }
            }
        }
    }
}
