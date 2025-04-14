// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.MongoDB.Tests;

public class MongoDBFeatureStoreTests : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly IFeatureStore _featureStore;

    public MongoDBFeatureStoreTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _featureStore = _scope.ServiceProvider.GetRequiredService<IFeatureStore>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _scope?.Dispose();
    }

    [Fact]
    public async Task GetFeatureAsync_ReturnsFeature_WhenFeatureExists()
    {
        // Act
        var feature = await _featureStore.GetFeatureAsync(FirstFeature);

        // Assert
        Assert.True(feature is not null);
        Assert.Equal(FirstFeature, feature.Name);
        Assert.NotEmpty(feature.Settings);
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
    }

    [Fact]
    public async Task GetFeatureAsync_ReturnsNull_WhenFeatureDoesNotExist()
    {
        // Arrange
        const string FeatureName = "NonExistentFeature";

        // Act/Assert
        Assert.Null(await _featureStore.GetFeatureAsync(FeatureName));
    }

    [Fact]
    public async Task GetFeaturesAsync_ReturnsAllFeatures()
    {
        // Act
        var result = await _featureStore.GetFeaturesAsync();

        // Assert
        Assert.True(result.Count > 0);
        Assert.Contains(result, f => f.Name == FirstFeature);
        Assert.Contains(result, f => f.Name == SecondFeature);
    }

    [Fact]
    public async Task CreateFeatureAsync_CreatesFeatureSuccessfully()
    {
        // Arrange
        var newFeature = new Feature
        {
            Id = Guid.NewGuid(),
            Name = FeatureToCreate,
            RequirementType = RequirementType.All,
            Settings =
            [
                new FeatureSettings
                {
                    Id = Guid.NewGuid(),
                    FilterType = FeatureFilterType.TimeWindow,
                    Parameters = "{\"start\":\"2023-01-01\",\"end\":\"2023-12-31\"}"
                }
            ]
        };

        // Act
        var createdFeature = await _featureStore.CreateFeatureAsync(newFeature);

        // Assert
        Assert.NotNull(createdFeature);
        Assert.Equal(newFeature.Name, createdFeature.Name);
        Assert.NotEmpty(createdFeature.Settings);
        Assert.Equal(newFeature.Settings.First().FilterType, createdFeature.Settings.First().FilterType);
    }

    [Fact]
    public async Task UpdateFeatureAsync_UpdatesFeatureSuccessfully()
    {
        // Arrange
        var existingFeature = await _featureStore.GetFeatureAsync(FeatureToUpdate);
        existingFeature.Name = "UpdatedFeatureName";

        // Act
        var updatedFeature = await _featureStore.UpdateFeatureAsync(existingFeature);

        // Assert
        Assert.NotNull(updatedFeature);
        Assert.Equal("UpdatedFeatureName", updatedFeature.Name);
    }

    [Fact]
    public async Task DeleteFeatureAsync_DeletesFeatureSuccessfully()
    {
        // Arrange
        var featureToDelete = await _featureStore.GetFeatureAsync(FeatureToDelete);

        // Act
        await _featureStore.DeleteFeatureAsync(featureToDelete.Id);

        // Assert
        var deletedFeature = await _featureStore.GetFeatureAsync(featureToDelete.Id);
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
            FeatureId = feature.Id,
            FilterType = FeatureFilterType.Percentage,
            Parameters = "{\"percentage\":50}"
        };

        // Act
        var createdSetting = await _featureStore.CreateFeatureSettingAsync(newSetting);

        // Assert
        Assert.NotNull(createdSetting);
        Assert.Equal(newSetting.FilterType, createdSetting.FilterType);
        Assert.Equal(newSetting.Parameters, createdSetting.Parameters);
    }

    [Fact]
    public async Task UpdateFeatureSettingAsync_UpdatesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(FirstFeature);
        var existingSetting = feature.Settings.First();
        existingSetting.Parameters = "{\"percentage\":75}";

        // Act
        var updatedSetting = await _featureStore.UpdateFeatureSettingAsync(existingSetting);

        // Assert
        Assert.NotNull(updatedSetting);
        Assert.Equal("{\"percentage\":75}", updatedSetting.Parameters);
    }

    [Fact]
    public async Task DeleteFeatureSettingAsync_DeletesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(SecondFeature);
        var settingToDelete = feature.Settings.First();

        // Act
        await _featureStore.DeleteFeatureSettingAsync(settingToDelete.Id);

        // Assert
        var deletedSetting = await _featureStore.GetFeatureSettingAsync(settingToDelete.Id);
        Assert.Null(deletedSetting);
    }
}