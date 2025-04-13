// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using static FeatureManagement.Database.Abstractions.Features;

#if NET9_0_OR_GREATER
using Microsoft.Extensions.Caching.Hybrid;
#else

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

#endif

namespace FeatureManagement.Database.Abstractions.Tests;

public class CachedFeatureStoreTests
{
    [Fact]
    public async Task GetFeatureFromCachedStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        // Act
        var feature = await cachedFeatureStore.GetFeatureAsync(FirstFeature);
#if NET9_0_OR_GREATER
        var cachedFeature = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + FirstFeature, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var cachedFeature = JsonSerializer.Deserialize<Feature>(await cache.GetAsync(FeatureCacheOptions.CachePrefix + FirstFeature));
#endif

        // Assert
        Assert.True(feature is not null);
        Assert.True(cachedFeature is not null);

        Assert.Equal(FirstFeature, feature.Name);
        Assert.Equal(feature.Name, cachedFeature.Name);

        Assert.True(feature.Settings.Count > 0);
        Assert.Equivalent(feature.Settings, cachedFeature.Settings);

        Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
        Assert.Equal(feature.Settings.First().FilterType, cachedFeature.Settings.First().FilterType);
    }

    [Fact]
    public async Task GetAllFeaturesFromCachedStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif
        var cacheOptions = serviceProvider.GetRequiredService<IOptions<FeatureCacheOptions>>().Value;

        // Act
        var features = await cachedFeatureStore.GetFeaturesAsync();
#if NET9_0_OR_GREATER
        var cachedFeatures = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + cacheOptions.KeyNames.AllFeatures, static (_) => ValueTask.FromResult(default(IReadOnlyCollection<Feature>)));
#else
        var cachedFeatures = JsonSerializer.Deserialize<IReadOnlyCollection<Feature>>(await cache.GetAsync(FeatureCacheOptions.CachePrefix + cacheOptions.KeyNames.AllFeatures));
#endif

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

            Assert.True(feature.Settings.Count > 0);
            Assert.Equivalent(feature.Settings, cachedFeature.Settings);

            Assert.Equal(FeatureFilterType.TimeWindow, feature.Settings.First().FilterType);
            Assert.Equal(feature.Settings.First().FilterType, cachedFeature.Settings.First().FilterType);
        }
    }

    [Fact]
    public async Task GetFeatureFromCacheMustBeNullWhenCacheExpires()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        // Act
        var feature = await cachedFeatureStore.GetFeatureAsync(FirstFeature);
        await Task.Delay(1200);
#if NET9_0_OR_GREATER
        var featureCache = await cache.GetOrCreateAsync(FirstFeature, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var featureCache = await cache.GetAsync(FirstFeature);
#endif

        // Assert
        Assert.True(feature is not null);
        Assert.True(featureCache is null);
    }

    [Fact]
    public async Task GetAllFeaturesFromCachedStoreWhenFeaturesHaveCircularReference()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStoreWithCircularReference>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        // Act
        var feature = await cachedFeatureStore.GetFeatureAsync(FirstFeature);
#if NET9_0_OR_GREATER
        var cachedFeature = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + FirstFeature, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var cachedFeature = JsonSerializer.Deserialize<Feature>(await cache.GetAsync(FeatureCacheOptions.CachePrefix + FirstFeature));
#endif

        // Assert
        Assert.True(feature is not null);
        Assert.True(cachedFeature is not null);

        Assert.Equal(FirstFeature, feature.Name);
        Assert.Equal(feature.Name, cachedFeature.Name);

        Assert.True(feature.Settings.Count > 0);
    }

    [Fact]
    public async Task UpdateFeatureInvalidatesCache()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        var feature = new Feature
        {
            Id = FeatureStore._firstFeatureId,
            Name = "TestFeature",
            Settings =
            [
                new FeatureSettings
                {
                    Id = Guid.NewGuid(),
                    FilterType = FeatureFilterType.TimeWindow,
                    Parameters = "{}"
                }
            ]
        };

        // Act
        await cachedFeatureStore.UpdateFeatureAsync(feature);
#if NET9_0_OR_GREATER
        var cachedFeature = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + feature.Name, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var cachedFeature = await cache.GetAsync(FeatureCacheOptions.CachePrefix + feature.Name);
#endif

        // Assert
        Assert.True(cachedFeature is null);
    }

    [Fact]
    public async Task DeleteFeatureInvalidatesCache()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        var feature = await cachedFeatureStore.GetFeatureAsync(FeatureStore._firstFeatureId);

        // Act
        await cachedFeatureStore.DeleteFeatureAsync(feature.Id);
#if NET9_0_OR_GREATER
        var cachedFeature = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + feature.Name, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var cachedFeature = await cache.GetAsync(FeatureCacheOptions.CachePrefix + feature.Name);
#endif

        // Assert
        Assert.True(cachedFeature is null);
    }

    [Fact]
    public async Task CreateFeatureSettingInvalidatesFeatureCache()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        var featureSetting = new FeatureSettings
        {
            Id = Guid.NewGuid(),
            FeatureId = FeatureStore._firstFeatureId,
            FilterType = FeatureFilterType.TimeWindow,
            Parameters = "{}"
        };

        // Act
        await cachedFeatureStore.CreateFeatureSettingAsync(featureSetting);
#if NET9_0_OR_GREATER
        var cachedFeature = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + FirstFeature, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var cachedFeature = await cache.GetAsync(FeatureCacheOptions.CachePrefix + FirstFeature);
#endif

        // Assert
        Assert.True(cachedFeature is null);
    }

    [Fact]
    public async Task DeleteFeatureSettingInvalidatesFeatureCache()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.ConfigureCachedFeatureStore();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var cachedFeatureStore = serviceProvider.GetRequiredService<IFeatureStore>();
#if NET9_0_OR_GREATER
        var cache = serviceProvider.GetRequiredService<HybridCache>();
#else
        var cache = serviceProvider.GetRequiredService<IDistributedCache>();
#endif

        var feature = await cachedFeatureStore.GetFeatureAsync(FeatureStore._firstFeatureId);
        var featureSettingId = feature.Settings.First().Id;

        // Act
        await cachedFeatureStore.DeleteFeatureSettingAsync(featureSettingId);
#if NET9_0_OR_GREATER
        var cachedFeature = await cache.GetOrCreateAsync(FeatureCacheOptions.CachePrefix + feature.Name, static (_) => ValueTask.FromResult(default(Feature)));
#else
        var cachedFeature = await cache.GetAsync(FeatureCacheOptions.CachePrefix + feature.Name);
#endif

        // Assert
        Assert.True(cachedFeature is null);
    }
}