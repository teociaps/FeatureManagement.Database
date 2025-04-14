// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.EntityFrameworkCore.Tests;

public abstract class EFCoreFeatureStoreTests<TWebApplicationFactory> : IClassFixture<TWebApplicationFactory>, IDisposable
    where TWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly IFeatureStore _featureStore;

    protected EFCoreFeatureStoreTests(TWebApplicationFactory factory)
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
    public async Task GetFeatureAsync_ThrowsException_WhenFeatureDoesNotExist()
    {
        // Arrange
        const string FeatureName = "NonExistentFeature";

        // Act/Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _featureStore.GetFeatureAsync(FeatureName));
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
            Settings = [
                new()
                {
                    Id = Guid.NewGuid(),
                    FilterType = FeatureFilterType.AlwaysOn,
                    Parameters = "{\"percentage\":50}"
                }]
        };

        // Act
        var createdFeature = await _featureStore.CreateFeatureAsync(newFeature);

        // Assert
        Assert.NotNull(createdFeature);
        Assert.Equal(newFeature.Name, createdFeature.Name);
        Assert.Equal(newFeature.RequirementType, createdFeature.RequirementType);
        Assert.Equal(newFeature.Settings.Count, createdFeature.Settings.Count);
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
        Assert.Null(await _featureStore.GetFeatureAsync(featureToDelete.Id));
    }

    [Fact]
    public async Task CreateFeatureSettingAsync_CreatesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(SecondFeature);
        var newFeatureSetting = new FeatureSettings
        {
            Id = Guid.NewGuid(),
            FeatureId = feature.Id,
            FilterType = FeatureFilterType.Percentage,
            Parameters = "{\"percentage\":50}"
        };

        // Act
        var createdFeatureSetting = await _featureStore.CreateFeatureSettingAsync(newFeatureSetting);

        // Assert
        Assert.NotNull(createdFeatureSetting);
        Assert.Equal(newFeatureSetting.FilterType, createdFeatureSetting.FilterType);
        Assert.Equal(newFeatureSetting.Parameters, createdFeatureSetting.Parameters);
    }

    [Fact]
    public async Task UpdateFeatureSettingAsync_UpdatesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(FirstFeature);
        var featureSetting = feature.Settings.First();
        featureSetting.Parameters = "{\"percentage\":75}";

        // Act
        var updatedFeatureSetting = await _featureStore.UpdateFeatureSettingAsync(featureSetting);

        // Assert
        Assert.NotNull(updatedFeatureSetting);
        Assert.Equal("{\"percentage\":75}", updatedFeatureSetting.Parameters);
    }

    [Fact]
    public async Task DeleteFeatureSettingAsync_DeletesFeatureSettingSuccessfully()
    {
        // Arrange
        var feature = await _featureStore.GetFeatureAsync(SecondFeature);
        var featureSettingToDelete = feature.Settings.First();

        // Act
        await _featureStore.DeleteFeatureSettingAsync(featureSettingToDelete.Id);

        // Assert
        Assert.Null(await _featureStore.GetFeatureSettingAsync(featureSettingToDelete.Id));
    }
}