// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace FeatureManagement.Database.EntityFrameworkCore;

/// <summary>
/// Represents the database context for managing features and their settings.
/// </summary>
public class FeatureManagementDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureManagementDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    protected FeatureManagementDbContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureManagementDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public FeatureManagementDbContext(DbContextOptions<FeatureManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> for features.
    /// </summary>
    public DbSet<Feature> Features { get; set; }

    /// <summary>
    /// The <see cref="DbSet{TEntity}"/> for feature settings.
    /// </summary>
    public DbSet<FeatureSettings> FeatureSettings { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeatureManagementDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
