// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.Tests;

public class FeatureStoreTests
{
    [Fact]
    public async Task GetFeatureFromStore()
    {
        // Arrange
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();

        // Act
        var feature = await featureStore.GetFeatureAsync(FirstFeature);

        // Assert
        Assert.True(feature is not null);
        Assert.Equal(FirstFeature, feature.Name);
        Assert.True(feature.Settings.Any());
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
    }

    [Fact]
    public async Task GetAllFeaturesFromStore()
    {
        // Arrange
        var featureStore = NSubstitute.Substitute.For<FeatureStore>();

        // Act
        var features = await featureStore.GetFeaturesAsync();

        // Assert
        Assert.True(features is not null);
        Assert.Equal(2, features.Count);
        Assert.NotEqual(features.First().Name, features.Last().Name);

        foreach (var feature in features)
        {
            Assert.True(feature is not null);

            Assert.True(feature.Settings.Any());
            Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
        }
    }
}