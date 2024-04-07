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

        var feature = await featureStore.GetFeatureAsync("TestFeature");

        Assert.True(feature is not null);

        var featureDefinition = await featureProvider.GetFeatureDefinitionAsync(feature.Name);

        Assert.True(featureDefinition is not null);

        Assert.True(featureDefinition.EnabledFor.Any());
        Assert.Equal(nameof(FeatureFilterType.TimeWindow), featureDefinition.EnabledFor.First().Name);
        Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", featureDefinition.EnabledFor.First().Parameters["Start"]);
        Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", featureDefinition.EnabledFor.First().Parameters["End"]);
    }
}