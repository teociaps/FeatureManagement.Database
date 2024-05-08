// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// A feature definition provider that pulls feature definitions from database.
/// </summary>
public sealed class DatabaseFeatureDefinitionProvider : IFeatureDefinitionProvider
{
    private readonly IFeatureStore _featureStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseFeatureDefinitionProvider"/> class.
    /// </summary>
    /// <param name="featureStore">The service used to get the feature definitions.</param>
    /// <exception cref="ArgumentNullException">Thrown if the feature store is not provided.</exception>
    public DatabaseFeatureDefinitionProvider(IFeatureStore featureStore)
    {
        _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
    }

    /// <inheritdoc/>
    public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        if (string.IsNullOrWhiteSpace(featureName))
            throw new ArgumentException($"The {nameof(featureName)} cannot be null or empty.", nameof(featureName));

        if (featureName.Contains(ConfigurationPath.KeyDelimiter))
            throw new ArgumentException($"The value '{ConfigurationPath.KeyDelimiter}' is not allowed in the feature name.", nameof(featureName));

        var feature = await _featureStore.GetFeatureAsync(featureName);
        return GetDefinitionFromFeature(feature);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        var features = await _featureStore.GetFeaturesAsync();
        foreach (var feature in features)
        {
            if (string.IsNullOrWhiteSpace(feature.Name))
                continue;

            yield return GetDefinitionFromFeature(feature);
        }
    }

    private static FeatureDefinition GetDefinitionFromFeature(Feature feature)
    {
        return new FeatureDefinition
        {
            Name = feature.Name,
            RequirementType = feature.RequirementType,
            EnabledFor = feature.Settings.Select(featureSetting =>
            {
                return new FeatureFilterConfiguration()
                {
                    Name = featureSetting.GetFilterType(),
                    Parameters = ConvertStringToConfiguration(featureSetting.Parameters)
                };
            })
        };
    }

    /// <summary>
    /// Transform string into <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="config">The string to convert.</param>
    /// <returns>The <see cref="IConfiguration"/>.</returns>
    private static IConfiguration ConvertStringToConfiguration(string config)
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Add(new JsonStringConfigurationSource(config));
        return configBuilder.Build();
    }
}
