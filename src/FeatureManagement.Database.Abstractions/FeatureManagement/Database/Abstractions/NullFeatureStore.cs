// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// Null object pattern implementation of <see cref="IFeatureStore"/>.
/// </summary>
public class NullFeatureStore : IFeatureStore
{
    /// <inheritdoc/>
    public Task<Feature> GetOrNullAsync([NotNull] string featureName)
    {
        return Task.FromResult(null as Feature);
    }
}