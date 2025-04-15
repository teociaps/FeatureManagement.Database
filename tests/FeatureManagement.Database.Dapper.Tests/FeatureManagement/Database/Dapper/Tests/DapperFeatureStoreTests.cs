// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.FeatureManagement;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.Dapper.Tests;

public class DapperFeatureStoreTests : IClassFixture<DapperFeatureStoreFixture>
{
    private readonly IFeatureStore _featureStore;

    public DapperFeatureStoreTests(DapperFeatureStoreFixture dapperFeatureStoreFixture)
    {
        _featureStore = dapperFeatureStoreFixture.FeatureStore;
    }

    [Fact]
    public async Task GetFeatureAsync_ReturnsFeature_WhenFeatureExists()
    {
        // Act
        var featureByName = await _featureStore.GetFeatureAsync(FirstFeature);

        // Assert
        Assert.NotNull(featureByName);
        Assert.Equal(FirstFeature, featureByName.Name);
    }

    [Fact]
    public async Task GetFeatureAsync_ReturnsFeature_WhenFeatureIdExists()
    {
        // Act
        var featureById = await _featureStore.GetFeatureAsync(DapperFeatureStoreFixture._firstFeatureId);

        // Assert
        Assert.NotNull(featureById);
        Assert.Equal(FirstFeature, featureById.Name);
    }

    [Fact]
    public async Task GetFeaturesAsync_ReturnsAllFeatures()
    {
        // Act
        var features = await _featureStore.GetFeaturesAsync();

        // Assert
        Assert.NotEmpty(features);
    }

    [Fact]
    public async Task CreateFeatureAsync_CreatesFeatureSuccessfully()
    {
        // Arrange
        var featureId = Guid.NewGuid();
        var newFeature = new Feature
        {
            Id = featureId,
            Name = FeatureToCreate,
            RequirementType = RequirementType.All,
            Settings =
            [
                new FeatureSettings
                {
                    Id = Guid.NewGuid(),
                    FilterType = FeatureFilterType.AlwaysOn,
                    FeatureId = featureId
                }
            ]
        };

        // Act
        var createdFeature = await _featureStore.CreateFeatureAsync(newFeature);

        // Assert
        Assert.NotNull(createdFeature);
        Assert.Equal(newFeature.Name, createdFeature.Name);
        Assert.NotEmpty(createdFeature.Settings);
    }

    [Fact]
    public async Task UpdateFeatureAsync_UpdatesFeatureSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(FeatureToUpdate);
        feature.Name = "UpdatedFeature";

        // Act
        var updatedFeature = await _featureStore.UpdateFeatureAsync(feature);

        // Assert
        Assert.NotNull(updatedFeature);
        Assert.Equal("UpdatedFeature", updatedFeature.Name);
    }

    [Fact]
    public async Task DeleteFeatureAsync_DeletesFeatureSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(FeatureToDelete);

        // Act
        await _featureStore.DeleteFeatureAsync(feature.Id);

        var deletedFeature = await _featureStore.GetFeatureAsync(FeatureToDelete);

        // Assert
        Assert.Null(deletedFeature);
    }

    [Fact]
    public async Task GetFeatureSettingAsync_ReturnsFeatureSetting_WhenSettingExists()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(FirstFeature);
        var featureSettingId = feature.Settings.First().Id;

        // Act
        var featureSetting = await _featureStore.GetFeatureSettingAsync(featureSettingId);

        // Assert
        Assert.NotNull(featureSetting);
        Assert.Equal(featureSettingId, featureSetting.Id);
    }

    [Fact]
    public async Task CreateFeatureSettingAsync_CreatesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(SecondFeature);
        var newSetting = new FeatureSettings
        {
            Id = Guid.NewGuid(),
            FilterType = FeatureFilterType.Percentage,
            Parameters = "50",
            FeatureId = feature.Id
        };

        // Act
        var createdSetting = await _featureStore.CreateFeatureSettingAsync(newSetting);

        // Assert
        Assert.NotNull(createdSetting);
        Assert.Equal(newSetting.FilterType, createdSetting.FilterType);
    }

    [Fact]
    public async Task UpdateFeatureSettingAsync_UpdatesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(FirstFeature);
        var setting = feature.Settings.First();
        setting.Parameters = "UpdatedParameters";

        // Act
        var updatedSetting = await _featureStore.UpdateFeatureSettingAsync(setting);

        // Assert
        Assert.NotNull(updatedSetting);
        Assert.Equal("UpdatedParameters", updatedSetting.Parameters);
    }

    [Fact]
    public async Task DeleteFeatureSettingAsync_DeletesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(SecondFeature);
        var setting = feature.Settings.First();

        // Act
        await _featureStore.DeleteFeatureSettingAsync(setting.Id);

        var deletedSetting = await _featureStore.GetFeatureSettingAsync(setting.Id);

        // Assert
        Assert.Null(deletedSetting);
    }
}