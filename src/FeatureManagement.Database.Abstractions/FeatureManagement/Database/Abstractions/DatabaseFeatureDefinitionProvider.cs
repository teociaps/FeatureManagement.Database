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
    protected ILogger Logger { get; init; }

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

        return _definitions.GetOrAdd(featureName, await ReadFeatureDefinitionFromStore(featureName));
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<FeatureDefinition> ReadFeatureDefinitionFromStore(string featureName)
    {
        var feature = await _featureStore.GetFeatureAsync(featureName);

        return new FeatureDefinition
        {
            Name = feature.Name,
            RequirementType = feature.RequirementType,
            EnabledFor = feature.Settings.Select(x =>
            {
                // Transform string into IConfiguration
                var configBuilder = new ConfigurationBuilder();
                var parsedDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(x.Parameters);
                configBuilder.AddInMemoryCollection(parsedDictionary.AsEnumerable());
                var configuration = configBuilder.Build();

                return new FeatureFilterConfiguration()
                {
                    Name = x.FilterType.ToString(),
                    Parameters = configuration
                };
            })
        };
    }
}