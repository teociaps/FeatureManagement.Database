// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using Microsoft.FeatureManagement;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring database feature management.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <paramref name="implementationType"/> for <see cref="IFeatureStore"/> to the service collection.
    /// </summary>
    /// <remarks>To use cache, see <see cref="AddCachedFeatureStore(IServiceCollection, Type)"/>.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationType">The implementation type of <see cref="IFeatureStore"/> to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="implementationType"/> not implements the <see cref="IFeatureStore"/> interface.</exception>
    public static IServiceCollection AddFeatureStore(this IServiceCollection services, Type implementationType)
    {
        implementationType ??= typeof(NullFeatureStore);

        if (typeof(IFeatureStore).IsAssignableFrom(implementationType))
            return services.AddScoped(typeof(IFeatureStore), implementationType);

        throw new ArgumentException(
            "The provided implementation type must inherits from IFeatureStore.",
            nameof(implementationType));
    }

    /// <summary>
    /// Adds an <typeparamref name="TFeatureStoreImplementation"/> for <see cref="IFeatureStore"/> to the service collection.
    /// </summary>
    /// <remarks>To use cache, see <see cref="AddCachedFeatureStore{TFeatureStoreImplementation}(IServiceCollection)"/>.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <typeparam name="TFeatureStoreImplementation">The implementation type of <see cref="IFeatureStore"/> to register.</typeparam>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <typeparamref name="TFeatureStoreImplementation"/> not implements the <see cref="IFeatureStore"/> interface.</exception>
    public static IServiceCollection AddFeatureStore<TFeatureStoreImplementation>(this IServiceCollection services)
        where TFeatureStoreImplementation : class, IFeatureStore
    {
        return services.AddFeatureStore(typeof(TFeatureStoreImplementation));
    }

    /// <summary>
    /// Adds an <paramref name="implementationType"/> for <see cref="IFeatureStore"/> to the service collection
    /// and register a service to manage features cache.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationType">The implementation type of <see cref="IFeatureStore"/> to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddCachedFeatureStore(this IServiceCollection services, Type implementationType)
    {
        services.AddFeatureStore(implementationType);

        services.AddOptions<FeatureCacheOptions>(FeatureCacheOptions.Name);
        services.AddDistributedMemoryCache();

        return services.Decorate<IFeatureStore, CachedFeatureStore>();
    }

    /// <summary>
    /// Adds an <typeparamref name="TFeatureStoreImplementation"/> for <see cref="IFeatureStore"/> to the service collection
    /// and register a service to manage features cache.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <typeparam name="TFeatureStoreImplementation">The implementation type of <see cref="IFeatureStore"/> to register.</typeparam>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddCachedFeatureStore<TFeatureStoreImplementation>(this IServiceCollection services)
        where TFeatureStoreImplementation : class, IFeatureStore
    {
        return services.AddCachedFeatureStore(typeof(TFeatureStoreImplementation));
    }

    /// <summary>
    /// Adds scoped <see cref="FeatureManager"/> and other required feature management services.
    /// </summary>
    /// <param name="services">The service collection that feature management services are added to.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="FeatureManagementException">Thrown if <see cref="FeatureManager"/> has been registered as singleton.</exception>
    public static IFeatureManagementBuilder AddDatabaseFeatureManagement(this IServiceCollection services)
    {
        CheckFeatureStoreRegistration(
            !services.Any(descriptor => descriptor.ServiceType == typeof(IFeatureStore)),
            errorMessage: "You need to register the IFeatureStore service.");

        services.AddDatabaseFeatureDefinitionProvider();

        return services.AddScopedFeatureManagement();
    }

    /// <summary>
    /// Adds scoped <see cref="FeatureManager"/> and other required feature management services.
    /// </summary>
    /// <param name="services">The service collection that feature management services are added to.</param>
    /// <param name="useCache">Indicates whether register a cache service to manage feature data. Default is <see langword="false"/>.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="FeatureManagementException">Thrown if <see cref="FeatureManager"/> has been registered as singleton.</exception>
    public static IFeatureManagementBuilder AddDatabaseFeatureManagement<TFeatureStoreImplementation>(this IServiceCollection services, bool useCache = false)
        where TFeatureStoreImplementation : class, IFeatureStore
    {
        CheckFeatureStoreRegistration(
            services.Any(descriptor => descriptor.ServiceType == typeof(IFeatureStore)),
            errorMessage: "You already registered the IFeatureStore service.");

        services = useCache
            ? services.AddCachedFeatureStore<TFeatureStoreImplementation>()
            : services.AddFeatureStore<TFeatureStoreImplementation>();

        services.AddDatabaseFeatureDefinitionProvider();

        return services.AddScopedFeatureManagement();
    }

    #region Private

    private static void CheckFeatureStoreRegistration(bool condition, string errorMessage)
    {
        if (condition)
            throw new FeatureManagementException(FeatureManagementError.Conflict, errorMessage);
    }

    private static void AddDatabaseFeatureDefinitionProvider(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddScoped<IFeatureDefinitionProvider, DatabaseFeatureDefinitionProvider>();
    }

    #endregion Private
}