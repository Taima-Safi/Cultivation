
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
    public DbSet<UserModel> User { get; set; }
    public DbSet<TokenModel> Token { get; set; }
    public DbSet<RoleModel> Role { get; set; }
    public DbSet<UserRoleModel> UserRole { get; set; }
    public DbSet<LandModel> Land { get; set; }
    public DbSet<ColorModel> Color { get; set; }
    public DbSet<FlowerModel> Flower { get; set; }
    public DbSet<CuttingModel> Cutting { get; set; }
    public DbSet<FertilizerModel> Fertilizer { get; set; }
    public DbSet<InsecticideModel> Insecticide { get; set; }
    public DbSet<CuttingLandModel> CuttingLand { get; set; }
    public DbSet<CuttingColorModel> CuttingColor { get; set; }
    public DbSet<FertilizerLandModel> FertilizerLand { get; set; }
    public DbSet<InsecticideLandModel> InsecticideLand { get; set; }
    public DbSet<OrderModel> Order { get; set; }
    public DbSet<ClientModel> Client { get; set; }
    public DbSet<OrderDetailModel> OrderDetail { get; set; }
    public DbSet<FlowerStoreModel> FlowerStore { get; set; }
    public DbSet<FertilizerMixModel> FertilizerMix { get; set; }
    public DbSet<FertilizerMixDetailModel> FertilizerMixDetail { get; set; }
    public DbSet<InsecticideMixModel> InsecticideMix { get; set; }
    public DbSet<InsecticideMixDetailModel> InsecticideMixDetail { get; set; }
    public DbSet<FertilizerMixLandModel> FertilizerMixLand { get; set; }
    public DbSet<InsecticideMixLandModel> InsecticideMixLand { get; set; }
    public DbSet<FertilizerStoreModel> FertilizerStore { get; set; }
    public DbSet<FertilizerTransactionModel> FertilizerTransaction { get; set; }
    public DbSet<FertilizerApplicableMixModel> FertilizerApplicableMix { get; set; }

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

        builder.Entity<FertilizerMixLandModel>()
    .HasOne(fm => fm.Land)
    .WithMany(l => l.FertilizerMixLands)
    .HasForeignKey(fm => fm.LandId)
    .OnDelete(DeleteBehavior.Cascade);
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
