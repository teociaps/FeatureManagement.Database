// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database.Tests;

// TODO: better reorganize tests

public class ServiceCollectionTests
{
    [Fact]
    public void RegisterDefaultNullFeatureStore()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFeatureStore(null);

        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));

        Assert.Equal(typeof(NullFeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterCustomFeatureStore()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddFeatureStore<FeatureStore>();

        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));

        Assert.Equal(typeof(FeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterFeatureStoreShouldThrowExceptionIfNotImplementsIFeatureStore()
    {
        var serviceCollection = new ServiceCollection();

        Assert.Throws<ArgumentException>("implementationType", () => serviceCollection.AddFeatureStore(typeof(Feature)));
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProvider()
    {
        var serviceCollection = new ServiceCollection();

        Assert.Throws<FeatureManagementException>(() => serviceCollection.AddDatabaseFeatureManagement());

        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.AddDatabaseFeatureManagement();

        var featureDefinitionProviderDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureDefinitionProvider));

        Assert.Equal(typeof(DatabaseFeatureDefinitionProvider), featureDefinitionProviderDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureDefinitionProviderDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderIncludingFeatureStore()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDatabaseFeatureManagement<FeatureStore>();

        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));

        Assert.Equal(typeof(FeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);

        var featureDefinitionProviderDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureDefinitionProvider));

        Assert.Equal(typeof(DatabaseFeatureDefinitionProvider), featureDefinitionProviderDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureDefinitionProviderDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderWithCachedService()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDatabaseFeatureManagement<FeatureStore>(useCache: true);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        AssertCachedFeatureStore(serviceCollection, serviceProvider);
    }

    [Fact]
    public void RegisterCachedFeatureStoreWithDefaultOptions()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddCachedFeatureStore<FeatureStore>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Default Options
        var featureCacheOptions = serviceProvider.GetRequiredService<IOptions<FeatureCacheOptions>>();

        Assert.True(featureCacheOptions is not null);
        Assert.True(featureCacheOptions.Value.AbsoluteExpiration is null);
        Assert.True(featureCacheOptions.Value.AbsoluteExpirationRelativeToNow is null);
        Assert.Equal(TimeSpan.FromMinutes(30), featureCacheOptions.Value.SlidingExpiration);
        Assert.True(featureCacheOptions.Value.KeyNames is not null);
        Assert.Equal("features", featureCacheOptions.Value.KeyNames.AllFeatures);

        // Cached Service
        AssertCachedFeatureStore(serviceCollection, serviceProvider);
    }

    [Fact]
    public void RegisterCachedFeatureStoreWithOptionsFromAppSettings()
    {
        var serviceCollection = new ServiceCollection();

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("FeatureManagement\\Database\\appsettings.json", false)
            .Build();

        serviceCollection.AddSingleton(config);
        serviceCollection.AddCachedFeatureStore<FeatureStore>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Options from appsettings.json
        var featureCacheOptions = serviceProvider.GetRequiredService<IOptions<FeatureCacheOptions>>();
        config.GetSection(FeatureCacheOptions.Name).Bind(featureCacheOptions.Value);

        Assert.True(featureCacheOptions is not null);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), featureCacheOptions.Value.AbsoluteExpiration);
        Assert.True(featureCacheOptions.Value.AbsoluteExpirationRelativeToNow is null);
        Assert.Equal(TimeSpan.FromMinutes(20), featureCacheOptions.Value.SlidingExpiration);
        Assert.True(featureCacheOptions.Value.KeyNames is not null);
        Assert.Equal("allFeatures", featureCacheOptions.Value.KeyNames.AllFeatures);

        // Cached Service
        AssertCachedFeatureStore(serviceCollection, serviceProvider);
    }

    private static void AssertCachedFeatureStore(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
    {
        var featureStoreDescriptors = serviceCollection.Where(s => s.ServiceType == typeof(IFeatureStore));

        Assert.Single(featureStoreDescriptors);

        var featureStore = featureStoreDescriptors.SingleOrDefault(d => d.ImplementationType is not null);
        var cachedFeatureStore = featureStoreDescriptors.SingleOrDefault(d => d.ImplementationFactory is not null);

        Assert.True(featureStore is null);
        Assert.True(cachedFeatureStore is not null);

        Assert.Equal(typeof(CachedFeatureStore), cachedFeatureStore.ImplementationFactory(serviceProvider).GetType());
        Assert.Equal(ServiceLifetime.Scoped, cachedFeatureStore.Lifetime);
    }
}