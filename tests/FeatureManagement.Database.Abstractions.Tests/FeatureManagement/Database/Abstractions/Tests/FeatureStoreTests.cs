// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using NSubstitute;
using static FeatureManagement.Database.Abstractions.Features;

namespace FeatureManagement.Database.Abstractions.Tests;

public class FeatureStoreTests
{
    [Fact]
    public async Task GetFeatureFromStore()
    {
        // Arrange
        var featureStore = Substitute.For<FeatureStore>();

        // Act
        var feature = await featureStore.GetFeatureAsync(FirstFeature);

        // Assert
        Assert.True(feature is not null);
        Assert.Equal(FirstFeature, feature.Name);
        Assert.True(feature.Settings.Count > 0);
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
    }

    [Fact]
    public async Task GetFeatureByIdFromStore()
    {
        // Arrange
        var featureStore = Substitute.For<FeatureStore>();
        var expectedFeature = await featureStore.GetFeatureAsync(FirstFeature);

        // Act
        var feature = await featureStore.GetFeatureAsync(expectedFeature.Id);

        // Assert
        Assert.NotNull(feature);
        Assert.Equal(expectedFeature.Id, feature.Id);
        Assert.Equal(expectedFeature.Name, feature.Name);
        Assert.True(feature.Settings.Count > 0);
        Assert.Equal(expectedFeature.Settings.First().FilterType, feature.Settings.First().FilterType);
    }

    [Fact]
    public async Task GetAllFeaturesFromStore()
    {
        // Arrange
        var featureStore = Substitute.For<FeatureStore>();

        // Act
        var features = await featureStore.GetFeaturesAsync();

        // Assert
        Assert.True(features is not null);
        Assert.Equal(2, features.Count);
        Assert.NotEqual(features.First().Name, features.Last().Name);

        foreach (var feature in features)
        {
            Assert.True(feature is not null);

            Assert.True(feature.Settings.Count > 0);
            Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
        }
    }

    [Fact]
    public async Task CreateFeatureInStore()
    {
        // Arrange
        var featureStore = new FeatureStore();
        var newFeature = new Feature
        {
            Name = "NewFeature",
            RequirementType = Microsoft.FeatureManagement.RequirementType.All,
            Settings = []
        };

        // Act
        var createdFeature = await featureStore.CreateFeatureAsync(newFeature);

        // Assert
        Assert.NotNull(createdFeature);
        Assert.Equal("NewFeature", createdFeature.Name);
        Assert.NotEqual(Guid.Empty, createdFeature.Id);
    }

    [Fact]
    public async Task UpdateFeatureInStore()
    {
        // Arrange
        var featureStore = new FeatureStore();
        var existingFeature = (await featureStore.GetFeaturesAsync()).First();
        existingFeature.Name = "UpdatedFeature";

        // Act
        var updatedFeature = await featureStore.UpdateFeatureAsync(existingFeature);

        // Assert
        Assert.NotNull(updatedFeature);
        Assert.Equal("UpdatedFeature", updatedFeature.Name);
    }

    [Fact]
    public async Task DeleteFeatureFromStore()
    {
        // Arrange
        var featureStore = new FeatureStore();
        var featureToDelete = (await featureStore.GetFeaturesAsync()).First();

        // Act
        await featureStore.DeleteFeatureAsync(featureToDelete.Id);
        var deletedFeature = await featureStore.GetFeatureAsync(featureToDelete.Id);

        // Assert
        Assert.Null(deletedFeature);
    }

    [Fact]
    public async Task CreateFeatureSettingInStore()
    {
        // Arrange
        var featureStore = new FeatureStore();
        var feature = (await featureStore.GetFeaturesAsync()).First();
        var newSetting = new FeatureSettings
        {
            FeatureId = feature.Id,
            FilterType = FeatureFilterType.Percentage,
            Parameters = """{"Percentage": 50}"""
        };

        // Act
        var createdSetting = await featureStore.CreateFeatureSettingAsync(newSetting);

        // Assert
        Assert.NotNull(createdSetting);
        Assert.Equal(FeatureFilterType.Percentage, createdSetting.FilterType);
        Assert.NotEqual(Guid.Empty, createdSetting.Id);
    }

    [Fact]
    public async Task UpdateFeatureSettingInStore()
    {
        // Arrange
        var featureStore = new FeatureStore();
        var feature = (await featureStore.GetFeaturesAsync()).First();
        var existingSetting = feature.Settings.First();
        existingSetting.Parameters = """{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sun, 01 Oct 2023 00:00:00 GMT"}""";

        // Act
        var updatedSetting = await featureStore.UpdateFeatureSettingAsync(existingSetting);

        // Assert
        Assert.NotNull(updatedSetting);
        Assert.Equal("""{"Start": "Mon, 01 May 2023 13:59:59 GMT", "End": "Sun, 01 Oct 2023 00:00:00 GMT"}""", updatedSetting.Parameters);
    }

    [Fact]
    public async Task DeleteFeatureSettingFromStore()
    {
        // Arrange
        var featureStore = new FeatureStore();
        var feature = (await featureStore.GetFeaturesAsync()).First();
        var settingToDelete = feature.Settings.First();

        // Act
        await featureStore.DeleteFeatureSettingAsync(settingToDelete.Id);
        var deletedSetting = await featureStore.GetFeatureSettingAsync(settingToDelete.Id);

        // Assert
        Assert.Null(deletedSetting);
    }
}