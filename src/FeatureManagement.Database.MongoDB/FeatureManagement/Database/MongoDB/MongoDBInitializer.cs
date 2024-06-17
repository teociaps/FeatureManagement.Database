// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using MongoDB.Driver;

namespace FeatureManagement.Database.MongoDB;

/// <summary>
/// Initializes MongoDB collections and their indexes.
/// </summary>
internal class MongoDBInitializer
{
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBInitializer"/> class with the specified MongoDB database.
    /// </summary>
    /// <param name="database">The MongoDB database instance.</param>
    internal MongoDBInitializer(IMongoDatabase database)
    {
        _database = database;
    }

    /// <summary>
    /// Initializes the MongoDB collections and their indexes.
    /// </summary>
    internal void Initialize()
    {
        var featureCollection = _database.GetCollection<Feature>("Features");
        var featureSettingsCollection = _database.GetCollection<FeatureSettings>("FeatureSettings");

        // Create unique index on Feature.Name
        var featureIndexKeysDefinition = Builders<Feature>.IndexKeys.Ascending(f => f.Name);
        var featureIndexOptions = new CreateIndexOptions { Unique = true };
        var featureIndexModel = new CreateIndexModel<Feature>(featureIndexKeysDefinition, featureIndexOptions);
        featureCollection.Indexes.CreateOne(featureIndexModel);

        // Create index on FeatureSettings.FeatureId
        var featureSettingsIndexKeysDefinition = Builders<FeatureSettings>.IndexKeys.Ascending(fs => fs.FeatureId);
        var featureSettingsIndexModel = new CreateIndexModel<FeatureSettings>(featureSettingsIndexKeysDefinition);
        featureSettingsCollection.Indexes.CreateOne(featureSettingsIndexModel);
    }
}
