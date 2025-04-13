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
    public virtual async Task<Feature> GetFeatureAsync(Guid featureId)
    {
        return await DbContext.Features.AsNoTracking().SingleOrDefaultAsync(f => f.Id == featureId);
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return await DbContext.Features.AsNoTracking().ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> CreateFeatureAsync([NotNull] Feature feature)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(feature);
#else
        if (feature is null)
            throw new ArgumentNullException(nameof(feature));
#endif
        DbContext.Features.Add(feature);
        await DbContext.SaveChangesAsync();
        return feature;
    }

    /// <inheritdoc/>
    public virtual async Task<Feature> UpdateFeatureAsync([NotNull] Feature feature)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(feature);
#else
        if (feature is null)
            throw new ArgumentNullException(nameof(feature));
#endif
        var existingFeature = await DbContext.Features.SingleOrDefaultAsync(f => f.Id == feature.Id)
        ?? throw new KeyNotFoundException($"Feature with ID '{feature.Id}' not found.");

        // Update feature properties
        existingFeature.Name = feature.Name;
        existingFeature.RequirementType = feature.RequirementType;

        DbContext.Features.Update(existingFeature);
        await DbContext.SaveChangesAsync();
        return existingFeature;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureAsync(Guid featureId)
    {
        var feature = await DbContext.Features.SingleOrDefaultAsync(f => f.Id == featureId)
            ?? throw new KeyNotFoundException($"Feature with ID '{featureId}' not found.");

        DbContext.Features.Remove(feature);
        await DbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        return await DbContext.FeatureSettings.AsNoTracking().SingleOrDefaultAsync(fs => fs.Id == featureSettingId);
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> CreateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(featureSetting);
#else
    if (featureSetting is null)
        throw new ArgumentNullException(nameof(featureSetting));
#endif
        DbContext.FeatureSettings.Add(featureSetting);
        await DbContext.SaveChangesAsync();
        return featureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureSettings> UpdateFeatureSettingAsync([NotNull] FeatureSettings featureSetting)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(featureSetting);
#else
        if (featureSetting is null)
            throw new ArgumentNullException(nameof(featureSetting));
#endif
        var existingFeatureSetting = await DbContext.FeatureSettings.SingleOrDefaultAsync(fs => fs.Id == featureSetting.Id)
            ?? throw new KeyNotFoundException($"Feature setting with ID '{featureSetting.Id}' not found.");

        existingFeatureSetting.FilterType = featureSetting.FilterType;
        existingFeatureSetting.CustomFilterTypeName = featureSetting.CustomFilterTypeName;
        existingFeatureSetting.Parameters = featureSetting.Parameters;
        existingFeatureSetting.FeatureId = featureSetting.FeatureId;

        DbContext.FeatureSettings.Update(existingFeatureSetting);
        await DbContext.SaveChangesAsync();
        return existingFeatureSetting;
    }

    /// <inheritdoc/>
    public virtual async Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        var featureSetting = await DbContext.FeatureSettings.SingleOrDefaultAsync(fs => fs.Id == featureSettingId)
            ?? throw new KeyNotFoundException($"Feature setting with ID '{featureSettingId}' not found.");

        DbContext.FeatureSettings.Remove(featureSetting);
        await DbContext.SaveChangesAsync();
    }
}