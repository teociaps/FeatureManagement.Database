// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// Null object pattern implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class NullFeatureStore : IFeatureStore
{
    private readonly IAsyncEnumerable<Feature> _emptyFeatures = AsyncEnumerable.Empty<Feature>();

    /// <inheritdoc/>
    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(null as Feature);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<Feature> GetFeaturesAsync()
    {
        await foreach (var feature in _emptyFeatures)
        {
            yield return feature;
        }
    }
}