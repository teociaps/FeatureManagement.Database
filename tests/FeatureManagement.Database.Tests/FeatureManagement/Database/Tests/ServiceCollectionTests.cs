// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database.Tests;

public class ServiceCollectionTests
{
    [Fact]
    public void RegisterDefaultNullFeatureStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddFeatureStore(null);
        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));

        // Assert
        Assert.Equal(typeof(NullFeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterCustomFeatureStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddFeatureStore<FeatureStore>();
        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));

        // Assert
        Assert.Equal(typeof(FeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterFeatureStoreShouldThrowExceptionIfNotImplementsIFeatureStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act/Assert
        Assert.Throws<ArgumentException>("implementationType", () => serviceCollection.AddFeatureStore(typeof(FeatureNonStore)));
    }
    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderShouldThrowExceptionIfStoreIsNotRegistered()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act/Assert
        Assert.Throws<FeatureManagementException>(() => serviceCollection.AddDatabaseFeatureManagement());
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderWithStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddFeatureStore<FeatureStore>();
        serviceCollection.AddDatabaseFeatureManagement();

        var featureDefinitionProviderDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureDefinitionProvider));

        // Assert
        Assert.Equal(typeof(DatabaseFeatureDefinitionProvider), featureDefinitionProviderDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureDefinitionProviderDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderIncludingFeatureStore()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddDatabaseFeatureManagement<FeatureStore>();

        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));
        var featureDefinitionProviderDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureDefinitionProvider));

        // Assert
        Assert.Equal(typeof(FeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);

        Assert.Equal(typeof(DatabaseFeatureDefinitionProvider), featureDefinitionProviderDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureDefinitionProviderDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderWithCachedService()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddDatabaseFeatureManagement<FeatureStore>(useCache: true);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        AssertCachedFeatureStore(serviceCollection, serviceProvider);
    }

    [Fact]
    public void RegisterCachedFeatureStoreWithDefaultOptions()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCachedFeatureStore<FeatureStore>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var featureCacheOptions = serviceProvider.GetRequiredService<IOptions<FeatureCacheOptions>>();

        // Assert
        Assert.True(featureCacheOptions is not null);
        Assert.True(featureCacheOptions.Value.AbsoluteExpiration is null);
        Assert.True(featureCacheOptions.Value.AbsoluteExpirationRelativeToNow is null);
        Assert.Equal(TimeSpan.FromMinutes(30), featureCacheOptions.Value.SlidingExpiration);
        Assert.True(featureCacheOptions.Value.KeyNames is not null);
        Assert.Equal("features", featureCacheOptions.Value.KeyNames.AllFeatures);

        AssertCachedFeatureStore(serviceCollection, serviceProvider);
    }

    [Fact]
    public void RegisterCachedFeatureStoreWithOptionsFromAppSettings()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("FeatureManagement/Database/appsettings.json", false)
            .Build();

        // Act
        serviceCollection.AddSingleton(config);
        serviceCollection.AddCachedFeatureStore<FeatureStore>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var featureCacheOptions = serviceProvider.GetRequiredService<IOptions<FeatureCacheOptions>>();
        config.GetSection(FeatureCacheOptions.Name).Bind(featureCacheOptions.Value);

        // Assert
        Assert.True(featureCacheOptions is not null);
        Assert.Equal(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), featureCacheOptions.Value.AbsoluteExpiration);
        Assert.True(featureCacheOptions.Value.AbsoluteExpirationRelativeToNow is null);
        Assert.Equal(TimeSpan.FromMinutes(20), featureCacheOptions.Value.SlidingExpiration);
        Assert.True(featureCacheOptions.Value.KeyNames is not null);
        Assert.Equal("allFeatures", featureCacheOptions.Value.KeyNames.AllFeatures);

        AssertCachedFeatureStore(serviceCollection, serviceProvider);
    }

    #region Private

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

    #endregion Private
}