// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Abstractions;

namespace FeatureManagement.Database.Tests;

public class DatabaseFeatureDefinitionProviderTests
{
    [Fact]
    public async Task GetFeatureDefinitionFromStore()
    {
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();
        var featureProvider = NSubstitute.Substitute.For<DatabaseFeatureDefinitionProvider>(featureStore);

        var feature = await featureStore.GetFeatureAsync("FirstFeature");

        Assert.True(feature is not null);

        var featureDefinition = await featureProvider.GetFeatureDefinitionAsync(feature.Name);

        Assert.True(featureDefinition is not null);

        Assert.True(featureDefinition.EnabledFor.Any());
        Assert.Equal(nameof(FeatureFilterType.TimeWindow), featureDefinition.EnabledFor.First().Name);
        Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", featureDefinition.EnabledFor.First().Parameters["Start"]);
        Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", featureDefinition.EnabledFor.First().Parameters["End"]);
    }

    [Fact]
    public async Task GetAllFeatureDefinitionsFromStore()
    {
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();
        var featureProvider = NSubstitute.Substitute.For<DatabaseFeatureDefinitionProvider>(featureStore);
        AsyncEnumerable.Empty<Feature>();
        var features = featureStore.GetFeaturesAsync();

        Assert.True(features is not null);
        Assert.Equal(2, await features.CountAsync());

        var featureDefinitions = featureProvider.GetAllFeatureDefinitionsAsync();

        Assert.NotEqual((await featureDefinitions.FirstAsync()).Name, (await featureDefinitions.LastAsync()).Name);

        await foreach (var featureDefinition in featureDefinitions)
        {
            Assert.True(featureDefinition is not null);

            Assert.True(featureDefinition.EnabledFor.Any());
            Assert.Equal(nameof(FeatureFilterType.TimeWindow), featureDefinition.EnabledFor.First().Name);
            Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", featureDefinition.EnabledFor.First().Parameters["Start"]);
            Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", featureDefinition.EnabledFor.First().Parameters["End"]);
        }
    }
}