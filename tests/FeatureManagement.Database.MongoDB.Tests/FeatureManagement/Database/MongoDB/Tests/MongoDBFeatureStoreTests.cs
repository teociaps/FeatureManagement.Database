// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

// Ignore Spelling: Mongo

using Microsoft.Extensions.DependencyInjection;
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
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Name == FirstFeature);
        Assert.Contains(result, f => f.Name == SecondFeature);
    }
}