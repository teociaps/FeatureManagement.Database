// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using static FeatureManagement.Database.Features;

namespace FeatureManagement.Database.Tests;

public class CachedFeatureStoreTests
{
    [Fact]
    public async Task GetFeatureFromCachedStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCachedFeatureStore<FeatureStore>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();

        // Act
        var feature = await cachedFeatureStore.GetFeatureAsync(FirstFeature);
        var cachedFeature = JsonSerializer.Deserialize<Feature>(await cache.GetAsync(FirstFeature));

        // Assert
        Assert.True(feature is not null);
        Assert.True(cachedFeature is not null);

        Assert.Equal(FirstFeature, feature.Name);
        Assert.Equal(feature.Name, cachedFeature.Name);
        Assert.True(feature.Settings.Any());
        Assert.Equivalent(feature.Settings, cachedFeature.Settings);
        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
        Assert.Equal(feature.Settings.First().FilterType, cachedFeature.Settings.First().FilterType);

        //Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", feature.Settings.First().Parameters["Start"]);
        //Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", feature.Settings.First().Parameters["End"]);
    }

    [Fact]
    public async Task GetAllFeaturesFromCachedStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCachedFeatureStore<FeatureStore>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
        var cacheOptions = serviceProvider.GetRequiredService<IOptions<FeatureCacheOptions>>().Value;

        // Act
        var features = await cachedFeatureStore.GetFeaturesAsync();
        var cachedFeatures = JsonSerializer.Deserialize<IReadOnlyCollection<Feature>>(await cache.GetAsync(cacheOptions.KeyNames.AllFeatures));

        // Assert
        Assert.True(features is not null);
        Assert.True(cachedFeatures is not null);

        Assert.Equal(2, features.Count);
        Assert.Equal(2, cachedFeatures.Count);

        Assert.NotEqual(features.First().Name, features.Last().Name);

        for (var i = 0; i < features.Count; i++)
        {
            var feature = features.ElementAt(i);
            var cachedFeature = cachedFeatures.ElementAt(i);

            Assert.True(feature is not null);
            Assert.True(cachedFeature is not null);

            Assert.Equal(feature.Name, cachedFeature.Name);
            Assert.True(feature.Settings.Any());
            Assert.Equivalent(feature.Settings, cachedFeature.Settings);
            Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
            Assert.Equal(feature.Settings.First().FilterType, cachedFeature.Settings.First().FilterType);
            //Assert.Equal("Mon, 01 May 2023 13:59:59 GMT", feature.Settings.First().Parameters["Start"]);
            //Assert.Equal("Sat, 01 July 2023 00:00:00 GMT", feature.Settings.First().Parameters["End"]);
        }
    }

    // TODO: test with expired cache must be null (set 2s expiration and wait 2s before check results)
}