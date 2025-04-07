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

        // IDs and Partition Keys
        const string Feature1Id = "7c81e846-dc77-4aff-bf03-8dd8bb2d3194";
        const string Feature2Id = "d3c82992-2f12-4008-9376-da37695a2747";
        const string FeatureSettingsId = "672dc1bd-9c5b-44ce-8461-234b262a8395";
        var partitionKey1 = new PartitionKey(FirstFeature);
        var partitionKey2 = new PartitionKey(SecondFeature);
        var featureSettingsPartitionKey = new PartitionKey(Feature1Id);

        try
        {
            // Delete existing items if they exist
            await featuresContainer.DeleteItemAsync<Feature>(Feature1Id, partitionKey1);
            await featuresContainer.DeleteItemAsync<Feature>(Feature2Id, partitionKey2);

            if (cosmosDBOptions.UseSeparateContainers)
                await featureSettingsContainer.DeleteItemAsync<FeatureSettings>(FeatureSettingsId, featureSettingsPartitionKey);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Item not found, nothing to delete
        }

        // Define new features and settings
        var features = new List<Feature>
        {
            new() {
                Id = Guid.Parse(Feature1Id),
                Name = FirstFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            },
            new() {
                Id = Guid.Parse(Feature2Id),
                Name = SecondFeature,
                RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            }
        };

        var settings = new List<FeatureSettings>
        {
            new() {
                Id = Guid.Parse(FeatureSettingsId),
                FeatureId = features[0].Id,
                FilterType = FeatureFilterType.TimeWindow,
                Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sat, 01 July 2023 00:00:00 GMT"}"""
            }
        };

        if (!cosmosDBOptions.UseSeparateContainers)
            features[0].Settings = settings;

        // Insert new items
        await featuresContainer.CreateItemAsync(new { id = features[0].Id.ToString(), features[0].Name, features[0].RequirementType, features[0].Settings }, partitionKey1);
        await featuresContainer.CreateItemAsync(new { id = features[1].Id.ToString(), features[1].Name, features[1].RequirementType }, partitionKey2);

        if (cosmosDBOptions.UseSeparateContainers)
            await featureSettingsContainer.CreateItemAsync(new { id = settings[0].Id.ToString(), settings[0].FeatureId, settings[0].FilterType, settings[0].Parameters }, featureSettingsPartitionKey);
    }
}