// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.FeatureManagement;
using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// Used to read features configured in a database.
/// </summary>
public interface IFeatureStore
{
    /// <summary>
    /// Gets a feature definition from database.
    /// </summary>
    /// <param name="featureName">The name of the feature to retrieve.</param>
    /// <returns>The feature definition or <see langword="null"/> if not found.</returns>
    Task<FeatureDefinition> GetOrNullAsync([NotNull] string featureName); // TODO: change FeatureDefinition return type
}