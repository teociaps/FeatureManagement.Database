// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// A feature definition provider that pulls feature definitions from database.
/// </summary>
public class DatabaseFeatureDefinitionProvider : IFeatureDefinitionProvider // TODO: register to DI
{
    private readonly IFeatureStore _featureStore;
    private readonly ConcurrentDictionary<string, FeatureDefinition> _definitions;

    /// <summary>
    /// The logger for the database feature definition provider.
    /// </summary>
    public ILogger Logger { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseFeatureDefinitionProvider"/> class.
    /// </summary>
    /// <param name="featureStore">The service used to get the feature definitions.</param>
    /// <exception cref="ArgumentNullException">Thrown when service is not provided.</exception>
    public DatabaseFeatureDefinitionProvider(IFeatureStore featureStore)
    {
        _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
        _definitions = new();
    }

    /// <inheritdoc/>
    public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        if (featureName is null)
            throw new ArgumentNullException(nameof(featureName));

        if (featureName.Contains(ConfigurationPath.KeyDelimiter))
            throw new ArgumentException($"The value '{ConfigurationPath.KeyDelimiter}' is not allowed in the feature name.", nameof(featureName));

        var feature = await _featureStore.GetFeatureAsync(featureName);
        return _definitions.GetOrAdd(featureName, GetDefinitionFromFeature(feature));
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        var features = await _featureStore.GetFeaturesAsync();
        foreach (var feature in features)
        {
            yield return _definitions.GetOrAdd(feature.Name, GetDefinitionFromFeature(feature));
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
        var parsedDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);
        configBuilder.AddInMemoryCollection(parsedDictionary.AsEnumerable());
        return configBuilder.Build();
    }
}