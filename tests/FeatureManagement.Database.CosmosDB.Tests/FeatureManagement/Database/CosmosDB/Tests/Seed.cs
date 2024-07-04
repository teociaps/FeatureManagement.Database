// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.CosmosDB.Tests;

internal static class Seed
{
    internal static async Task SeedDataAsync(CosmosDBOptions cosmosDBOptions, ICosmosDBConnectionFactory cosmosDBConnectionFactory)
    {
        var client = cosmosDBConnectionFactory.CreateClient();
        var database = await client.CreateDatabaseIfNotExistsAsync(cosmosDBOptions.DatabaseName);

        await database.Database.CreateContainerIfNotExistsAsync(cosmosDBOptions.FeaturesCollectionName, "/Name");

        if (cosmosDBOptions.UseSeparateContainers)
            await database.Database.CreateContainerIfNotExistsAsync(cosmosDBOptions.FeatureSettingsCollectionName, "/FeatureId");

        var featuresContainer = cosmosDBConnectionFactory.GetFeaturesContainer();
        var featureSettingsContainer = cosmosDBConnectionFactory.GetFeatureSettingsContainer();

        await featuresContainer.DeleteItemAsync<Feature>("7c81e846-dc77-4aff-bf03-8dd8bb2d3194", new PartitionKey(FirstFeature));
        await featuresContainer.DeleteItemAsync<Feature>("d3c82992-2f12-4008-9376-da37695a2747", new PartitionKey(SecondFeature));

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

        await featureSettingsContainer.DeleteItemAsync<FeatureSettings>("672dc1bd-9c5b-44ce-8461-234b262a8395", new PartitionKey("7c81e846-dc77-4aff-bf03-8dd8bb2d3194"));

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

        if (!cosmosDBOptions.UseSeparateContainers)
            features[0].Settings = settings;

        await featuresContainer.CreateItemAsync(new { id = features[0].Id.ToString(), features[0].Name, features[0].RequirementType, features[0].Settings }, new PartitionKey(features[0].Name));
        await featuresContainer.CreateItemAsync(new { id = features[1].Id.ToString(), features[1].Name, features[1].RequirementType }, new PartitionKey(features[1].Name));

        if (cosmosDBOptions.UseSeparateContainers)
            await featureSettingsContainer.CreateItemAsync(new { id = settings[0].Id.ToString(), settings[0].FeatureId, settings[0].FilterType, settings[0].CustomFilterTypeName, settings[0].Parameters }, new PartitionKey(settings[0].FeatureId.ToString()));
    }
}