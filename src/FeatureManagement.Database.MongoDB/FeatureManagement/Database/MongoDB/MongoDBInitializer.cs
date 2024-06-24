// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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

        // Define the index models
        var featureIndexKeysDefinition = Builders<Feature>.IndexKeys.Ascending(f => f.Name);
        var featureIndexOptions = new CreateIndexOptions { Unique = true };
        var featureIndexModel = new CreateIndexModel<Feature>(featureIndexKeysDefinition, featureIndexOptions);

        var featureSettingsIndexKeysDefinition = Builders<FeatureSettings>.IndexKeys.Ascending(fs => fs.FeatureId);
        var featureSettingsIndexModel = new CreateIndexModel<FeatureSettings>(featureSettingsIndexKeysDefinition);

        // Create indexes if they do not exist
        CreateIndexIfNotExists(featureCollection, featureIndexModel);
        CreateIndexIfNotExists(featureSettingsCollection, featureSettingsIndexModel);
    }

    private static void CreateIndexIfNotExists<TDocument>(IMongoCollection<TDocument> collection, CreateIndexModel<TDocument> indexModel)
    {
        var indexName = indexModel.Options?.Name ?? GenerateIndexName(indexModel);

        // List existing indexes
        var existingIndexes = collection.Indexes.List().ToList();
        var indexExists = existingIndexes.Exists(index => index["name"] == indexName);

        if (!indexExists)
        {
            collection.Indexes.CreateOne(indexModel);
        }
    }

    private static string GenerateIndexName<TDocument>(CreateIndexModel<TDocument> indexModel)
    {
        // Generate a name for the index based on the keys definition
        var indexKeys = indexModel.Keys.Render(BsonSerializer.SerializerRegistry.GetSerializer<TDocument>(), BsonSerializer.SerializerRegistry);
        var indexName = string.Join("_", indexKeys.ToBsonDocument().Elements.Select(e => $"{e.Name}_{e.Value.ToString().ToLower()}"));
        return indexName;
    }
}