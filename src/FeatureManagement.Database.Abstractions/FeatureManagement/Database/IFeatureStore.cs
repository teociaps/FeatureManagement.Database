// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database;

/// <summary>
/// Used to read features configured in a database.
/// </summary>
public interface IFeatureStore
{
    /// <summary>
    /// Gets a feature from database.
    /// </summary>
    /// <param name="featureName">The name of the feature to retrieve.</param>
    /// <returns>The feature.</returns>
    Task<Feature> GetFeatureAsync([NotNull] string featureName);

    /// <summary>
    /// Gets all features from database.
    /// </summary>
    /// <returns>A list of features.</returns>
    Task<IReadOnlyCollection<Feature>> GetFeaturesAsync();
}

// TODO: add cancellationToken