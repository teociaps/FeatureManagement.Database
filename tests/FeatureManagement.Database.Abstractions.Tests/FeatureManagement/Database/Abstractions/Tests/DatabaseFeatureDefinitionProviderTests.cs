// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using static FeatureManagement.Database.Abstractions.Features;

namespace FeatureManagement.Database.Abstractions.Tests;

public class DatabaseFeatureDefinitionProviderTests
{
    [Fact]
    public async Task GetFeatureDefinitionFromStore()
    {
        // Arrange
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();
        var featureProvider = new DatabaseFeatureDefinitionProvider(featureStore);

        // Act
        var featureDefinition = await featureProvider.GetFeatureDefinitionAsync(SecondFeature);

        // Assert
        Assert.True(featureDefinition is not null);
        Assert.True(featureDefinition.EnabledFor.Any());
        Assert.Equal(nameof(FeatureFilterType.TimeWindow), featureDefinition.EnabledFor.First().Name);
        Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", featureDefinition.EnabledFor.First().Parameters["Start"]);
        Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", featureDefinition.EnabledFor.First().Parameters["End"]);
    }

    [Fact]
    public async Task GetAllFeatureDefinitionsFromStore()
    {
        // Arrange
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();
        var featureProvider = new DatabaseFeatureDefinitionProvider(featureStore);

        // Act
        var featureDefinitions = featureProvider.GetAllFeatureDefinitionsAsync();

        // Assert
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