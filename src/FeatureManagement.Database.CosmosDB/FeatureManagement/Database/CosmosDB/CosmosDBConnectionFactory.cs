// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FeatureManagement.Database.CosmosDB;

/// <summary>
/// Default implementation of <see cref="ICosmosDBConnectionFactory"/>.
/// </summary>
public class CosmosDBConnectionFactory : ICosmosDBConnectionFactory
{
    private readonly CosmosClient _client;
    private readonly Container _featuresContainer;
    private readonly Container _featureSettingsContainer;
    private readonly bool _useSeparateContainers;

    /// <summary>
    /// Gets the Cosmos DB configuration options.
    /// </summary>
    protected readonly CosmosDBOptions CosmosDBOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDBConnectionFactory"/> class.
    /// </summary>
    /// <param name="options">The Cosmos DB options.</param>
    public CosmosDBConnectionFactory(IOptions<CosmosDBOptions> options)
    {
        CosmosDBOptions = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _client = new CosmosClient(CosmosDBOptions.EndpointUri, CosmosDBOptions.AccountKey);
        _useSeparateContainers = CosmosDBOptions.UseSeparateContainers;

        if (_useSeparateContainers)
        {
            _featuresContainer = _client.GetContainer(CosmosDBOptions.DatabaseName, CosmosDBOptions.FeaturesCollectionName);
            _featureSettingsContainer = _client.GetContainer(CosmosDBOptions.DatabaseName, CosmosDBOptions.FeatureSettingsCollectionName);
        }
        else
        {
            _featuresContainer = _client.GetContainer(CosmosDBOptions.DatabaseName, CosmosDBOptions.FeaturesCollectionName);
        }
    }

    /// <inheritdoc/>
    public virtual CosmosClient CreateClient()
    {
        return _client;
    }

    /// <inheritdoc/>
    public virtual Container GetFeaturesContainer()
    {
        return _featuresContainer;
    }

    /// <inheritdoc/>
    public virtual Container GetFeatureSettingsContainer()
    {
        if (_useSeparateContainers)
        {
            return _featureSettingsContainer;
        }
        return _featuresContainer; // Return the features container if using a single container
    }
}