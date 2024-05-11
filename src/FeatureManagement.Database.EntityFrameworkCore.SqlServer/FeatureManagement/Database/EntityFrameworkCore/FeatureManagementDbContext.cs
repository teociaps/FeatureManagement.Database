using Microsoft.EntityFrameworkCore;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class FeatureManagementDbContext : DbContext
{
    public FeatureManagementDbContext()
    {
    }

    public FeatureManagementDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }
    public DbSet<Feature> Features { get; set; }

    public DbSet<FeatureSettings> FeatureSettings { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeatureManagementDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}