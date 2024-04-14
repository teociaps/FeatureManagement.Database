using FeatureManagement.Database.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database.Tests;

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
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var featureDefinitionProviderDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureDefinitionProvider));

        Assert.Equal(typeof(DatabaseFeatureDefinitionProvider), featureDefinitionProviderDescriptor.ImplementationFactory(serviceProvider).GetType());
        Assert.Equal(ServiceLifetime.Singleton, featureDefinitionProviderDescriptor.Lifetime);
    }

    [Fact]
    public void RegisterDatabaseFeatureDefinitionProviderWithStoreIncluded()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDatabaseFeatureManagement<FeatureStore>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var featureStoreDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureStore));

        Assert.Equal(typeof(FeatureStore), featureStoreDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, featureStoreDescriptor.Lifetime);

        var featureDefinitionProviderDescriptor = serviceCollection.Single(s => s.ServiceType == typeof(IFeatureDefinitionProvider));

        Assert.Equal(typeof(DatabaseFeatureDefinitionProvider), featureDefinitionProviderDescriptor.ImplementationFactory(serviceProvider).GetType());
        Assert.Equal(ServiceLifetime.Singleton, featureDefinitionProviderDescriptor.Lifetime);
    }
}