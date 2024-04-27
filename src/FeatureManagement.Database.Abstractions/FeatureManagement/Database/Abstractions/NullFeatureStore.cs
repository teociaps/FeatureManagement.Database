// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// Null object pattern implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class NullFeatureStore : IFeatureStore
{
    private readonly IReadOnlyCollection<Feature> _emptyFeatures = [];

    /// <inheritdoc/>
    public Task<Feature> GetFeatureAsync([NotNull] string featureName)
    {
        return Task.FromResult(null as Feature);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyCollection<Feature>> GetFeaturesAsync()
    {
        return Task.FromResult(_emptyFeatures);
    }
}