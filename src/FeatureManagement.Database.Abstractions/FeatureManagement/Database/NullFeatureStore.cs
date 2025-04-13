// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database;

/// <summary>
/// Null object pattern implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class NullFeatureStore : IFeatureStore
{
    private readonly IReadOnlyCollection<Feature> _emptyFeatures = [];

    /// <inheritdoc/>
    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult<Feature>(null);
    }

    /// <inheritdoc/>
    public Task<Feature> GetFeatureAsync(Guid featureId)
    {
        return Task.FromResult<Feature>(null);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return Task.FromResult(_emptyFeatures);
    }

    /// <inheritdoc/>
    public Task<Feature> CreateFeatureAsync(Feature feature)
    {
        return Task.FromResult<Feature>(null);
    }

    /// <inheritdoc/>
    public Task<Feature> UpdateFeatureAsync(Feature feature)
    {
        return Task.FromResult<Feature>(null);
    }

    /// <inheritdoc/>
    public Task DeleteFeatureAsync(Guid featureId)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId)
    {
        return Task.FromResult<FeatureSettings>(null);
    }

    /// <inheritdoc/>
    public Task<FeatureSettings> CreateFeatureSettingAsync(FeatureSettings featureSetting)
    {
        return Task.FromResult<FeatureSettings>(null);
    }

    /// <inheritdoc/>
    public Task<FeatureSettings> UpdateFeatureSettingAsync(FeatureSettings featureSetting)
    {
        return Task.FromResult<FeatureSettings>(null);
    }

    /// <inheritdoc/>
    public Task DeleteFeatureSettingAsync(Guid featureSettingId)
    {
        return Task.CompletedTask;
    }
}
