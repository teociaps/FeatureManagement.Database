// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class FeatureStore : IFeatureStore
{
    /// <summary>
    /// The <see cref="FeatureManagementDbContext"/> for accessing the database.
    /// </summary>
    protected readonly FeatureManagementDbContext DbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStore"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="FeatureManagementDbContext"/> to be used by this service.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbContext"/> is null.</exception>
    public FeatureStore(FeatureManagementDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        if (string.IsNullOrEmpty(featureName))
            throw new ArgumentNullException(nameof(featureName));

        var feature = await DbContext.Features.AsNoTracking().SingleOrDefaultAsync(f => f.Name == featureName)
            ?? throw new KeyNotFoundException($"Feature with name '{featureName}' not found.");

        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return await DbContext.Features.AsNoTracking().ToListAsync();
    }
}