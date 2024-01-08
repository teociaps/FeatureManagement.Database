// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.FeatureManagement;

namespace FeatureManagement.Database.Abstractions;

/// <summary>
/// A feature definition provider that pulls feature definitions from database.
/// </summary>
public class DatabaseFeatureDefinitionProvider : IFeatureDefinitionProvider
{
    private readonly IFeatureStore _featureStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseFeatureDefinitionProvider"/> class.
    /// </summary>
    /// <param name="featureStore">The service used to get the feature definitions.</param>
    /// <exception cref="ArgumentNullException">Thrown when service is not provided.</exception>
    public DatabaseFeatureDefinitionProvider(IFeatureStore featureStore)
    {
        _featureStore = featureStore ?? throw new ArgumentNullException(nameof(featureStore));
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        throw new NotImplementedException();
    }
}