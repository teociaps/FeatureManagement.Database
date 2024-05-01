// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// A feature definition provider that pulls feature definitions from database.
/// </summary>
public class DatabaseFeatureDefinitionProvider : IFeatureDefinitionProvider
{
    private readonly IFeatureStore _featureStore;

    /// <summary>
    /// The logger for the database feature definition provider.
    /// </summary>
    protected ILogger<DatabaseFeatureDefinitionProvider> Logger { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseFeatureDefinitionProvider"/> class.
    /// </summary>
    /// <param name="featureStore">The service used to get the feature definitions.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when any service is not provided.</exception>
    public DatabaseFeatureDefinitionProvider(IFeatureStore featureStore, ILogger<DatabaseFeatureDefinitionProvider> logger)
    {
        _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public virtual async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        if (featureName is null)
            throw new ArgumentNullException(nameof(featureName));

        if (featureName.Contains(ConfigurationPath.KeyDelimiter))
            throw new ArgumentException($"The value '{ConfigurationPath.KeyDelimiter}' is not allowed in the feature name.", nameof(featureName));

        var feature = await _featureStore.GetFeatureAsync(featureName);
        return GetDefinitionFromFeature(feature);
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        var features = await _featureStore.GetFeaturesAsync();
        foreach (var feature in features)
        {
            yield return GetDefinitionFromFeature(feature);
        }
    }

    private static FeatureDefinition GetDefinitionFromFeature(Feature feature)
    {
        return new FeatureDefinition
        {
            Name = feature.Name,
            RequirementType = feature.RequirementType,
            EnabledFor = feature.Settings.Select(x =>
            {
                return new FeatureFilterConfiguration()
                {
                    Name = x.FilterType.ToString(),
                    Parameters = ConvertStringToConfiguration(x.Parameters)
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
