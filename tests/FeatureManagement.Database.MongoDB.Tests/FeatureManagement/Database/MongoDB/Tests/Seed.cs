// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.MongoDB.Tests;

internal static class Seed
{
    internal static async Task SeedDataAsync(IMongoDatabase database)
    {
        // Ensure the GuidSerializer is registered
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var featureCollection = database.GetCollection<Feature>("Features");
        var featureSettingsCollection = database.GetCollection<FeatureSettings>("FeatureSettings");

        // Clean the collections if they already exist
        await database.DropCollectionAsync("Features");
        await database.DropCollectionAsync("FeatureSettings");

        List<Feature> features =
        [
            new()
            {
                Id = Guid.Parse("7C81E846-DC77-4AFF-BF03-8DD8BB2D3194"),
                Name = FirstFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            },
            new()
            {
                Id = Guid.Parse("D3C82992-2F12-4008-9376-DA37695A2747"),
                Name = SecondFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            }
        ];

        List<FeatureSettings> settings =
        [
            new()
            {
                Id = Guid.Parse("672DC1BD-9C5B-44CE-8461-234B262A8395"),
                FeatureId = features[0].Id,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            }
        ];

        await featureCollection.InsertManyAsync(features);
        await featureSettingsCollection.InsertManyAsync(settings);
    }
}
