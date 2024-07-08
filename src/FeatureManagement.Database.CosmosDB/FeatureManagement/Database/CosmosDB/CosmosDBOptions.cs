// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database.CosmosDB;

/// <summary>
/// Represents the configuration options for CosmosDB.
/// </summary>
public class CosmosDBOptions
{
    /// <summary>
    /// The endpoint URI for the Cosmos DB account.
    /// </summary>
    public string EndpointUri { get; set; }

    /// <summary>
    /// The authentication key or resource token for the Cosmos DB account.
    /// </summary>
    public string AccountKey { get; set; }

    /// <summary>
    /// The name of the Cosmos DB database.
    /// </summary>
    public string DatabaseName { get; set; }

    /// <summary>
    /// The name of the Cosmos DB collection for features.
    /// By default is <c>"Features"</c>.
    /// </summary>
    public string FeaturesCollectionName { get; set; } = "Features";

    /// <summary>
    /// The name of the Cosmos DB collection for feature settings.
    /// By default is <c>"FeatureSettings"</c>.
    /// </summary>
    public string FeatureSettingsCollectionName { get; set; } = "FeatureSettings";

    /// <summary>
    /// Use separate containers for features and feature settings.
    /// <para />
    /// By default this option is set to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// If you decide to merge them into a single container, set this to <see langword="false"/>.
    /// Merging them into a single container depends on several factors including your querying patterns,
    /// data access requirements, and performance considerations.
    /// When opting for a single container, <see cref="FeaturesCollectionName"/> will be used with
    /// the related container.
    /// </remarks>
    public bool UseSeparateContainers { get; set; } = true;
}