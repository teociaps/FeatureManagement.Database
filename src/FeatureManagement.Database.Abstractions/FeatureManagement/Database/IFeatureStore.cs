// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace FeatureManagement.Database;

/// <summary>
/// Provides operations for managing features configured in a database.
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
    /// Gets a feature by its Id from the database.
    /// </summary>
    /// <param name="featureId">The Id of the feature to retrieve.</param>
    /// <returns>The feature.</returns>
    Task<Feature> GetFeatureAsync(Guid featureId);

    /// <summary>
    /// Gets all features from database.
    /// </summary>
    /// <returns>A list of features.</returns>
    Task<IReadOnlyCollection<Feature>> GetFeaturesAsync();

    /// <summary>
    /// Creates a new feature in the database and returns the created feature.
    /// </summary>
    /// <param name="feature">The feature to create.</param>
    /// <returns>The created feature.</returns>
    Task<Feature> CreateFeatureAsync([NotNull] Feature feature);

    /// <summary>
    /// Updates an existing feature in the database and returns the updated feature.
    /// </summary>
    /// <param name="feature">The feature to update.</param>
    /// <returns>The updated feature.</returns>
    Task<Feature> UpdateFeatureAsync([NotNull] Feature feature);

    /// <summary>
    /// Deletes a feature from the database.
    /// </summary>
    /// <param name="featureId">The Id of the feature to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteFeatureAsync(Guid featureId);

    /// <summary>
    /// Gets a feature setting by its Id from the database.
    /// </summary>
    /// <param name="featureSettingId">The Id of the feature setting to retrieve.</param>
    /// <returns>The feature setting.</returns>
    Task<FeatureSettings> GetFeatureSettingAsync(Guid featureSettingId);

    /// <summary>
    /// Creates a new feature setting in the database and returns the created setting.
    /// </summary>
    /// <param name="featureSetting">The feature setting to create.</param>
    /// <returns>The created feature setting.</returns>
    Task<FeatureSettings> CreateFeatureSettingAsync([NotNull] FeatureSettings featureSetting);

    /// <summary>
    /// Updates an existing feature setting in the database and returns the updated setting.
    /// </summary>
    /// <param name="featureSetting">The feature setting to update.</param>
    /// <returns>The updated feature setting.</returns>
    Task<FeatureSettings> UpdateFeatureSettingAsync([NotNull] FeatureSettings featureSetting);

    /// <summary>
    /// Deletes a feature setting from the database.
    /// </summary>
    /// <param name="featureSettingId">The Id of the feature setting to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteFeatureSettingAsync(Guid featureSettingId);
}

// TODO: add cancellationToken