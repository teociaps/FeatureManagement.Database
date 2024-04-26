// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.Tests;

public class FeatureStoreTests
{
    [Fact]
    public async Task GetFeatureFromStore()
    {
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();

        var feature = await featureStore.GetFeatureAsync(FirstFeature);

        Assert.True(feature is not null);

        Assert.Equal(FirstFeature, feature.Name);
        Assert.True(feature.Settings.Any());
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
        //Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", feature.Settings.First().Parameters["Start"]);
        //Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", feature.Settings.First().Parameters["End"]);
    }

    [Fact]
    public async Task GetAllFeaturesFromStore()
    {
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();

        var features = await featureStore.GetFeaturesAsync();

        Assert.True(features is not null);
        Assert.Equal(2, features.Count);
        Assert.NotEqual(features.First().Name, features.Last().Name);

        foreach (var feature in features)
        {
            Assert.True(feature is not null);

            Assert.True(feature.Settings.Any());
            Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
            //Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", feature.Settings.First().Parameters["Start"]);
            //Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", feature.Settings.First().Parameters["End"]);
        }
    }
}