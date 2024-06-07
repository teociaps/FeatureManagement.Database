// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Name == FirstFeature);
        Assert.Contains(result, f => f.Name == SecondFeature);
    }
}