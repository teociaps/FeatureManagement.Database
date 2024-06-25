// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;

namespace FeatureManagement.Database.CosmosDB;

/// <summary>
/// Defines a factory for creating Cosmos DB client and container instances.
/// </summary>
public interface ICosmosDBConnectionFactory
{
    /// <summary>
    /// Creates a new <see cref="CosmosClient"/> instance.
    /// </summary>
    /// <returns>A new <see cref="CosmosClient"/> instance.</returns>
    CosmosClient CreateClient();

    /// <summary>
    /// Gets the Cosmos DB container for features.
    /// </summary>
    /// <returns>The Cosmos DB container for features.</returns>
    Container GetFeaturesContainer();

    /// <summary>
    /// Gets the Cosmos DB container for feature settings.
    /// </summary>
    /// <returns>The Cosmos DB container for feature settings.</returns>
    Container GetFeatureSettingsContainer();
}