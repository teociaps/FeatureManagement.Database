// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Azure.Cosmos;

namespace FeatureManagement.Database.CosmosDB.Tests;

using static FeatureManagement.Database.Features;

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

        await featuresContainer.CreateItemAsync(features[0], new PartitionKey(features[0].Name));
        await featuresContainer.CreateItemAsync(features[1], new PartitionKey(features[0].Name));

        await featureSettingsContainer.CreateItemAsync(settings[0], new PartitionKey(settings[0].FeatureId.ToString()));
    }
}