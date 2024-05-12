// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace FeatureManagement.Database.EntityFrameworkCore;

public class FeatureManagementDbContext : DbContext
{
    protected FeatureManagementDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public FeatureManagementDbContext(DbContextOptions<FeatureManagementDbContext> options)
        : base(options)
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