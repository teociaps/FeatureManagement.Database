// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementationType">The implementation type of <see cref="IFeatureStore"/> to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="implementationType"/> not implements the <see cref="IFeatureStore"/> interface.</exception>
    public static IServiceCollection AddFeatureStore(this IServiceCollection services, Type implementationType)
    {
        implementationType ??= typeof(NullFeatureStore);

        if (typeof(IFeatureStore).IsAssignableFrom(implementationType))
        {
            services.TryAddScoped(typeof(IFeatureStore), implementationType);
            return services;
        }

        throw new ArgumentException(
            "The provided implementation type must inherits from IFeatureStore.",
            nameof(implementationType));
    }

    /// <summary>
    /// Adds an <typeparamref name="TFeatureStore"/> for <see cref="IFeatureStore"/> to the service collection.
    /// </summary>
    /// <typeparam name="TFeatureStore">The service type used to retrieve data from database.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided <typeparamref name="TFeatureStore"/> not implements the <see cref="IFeatureStore"/> interface.</exception>
    public static IServiceCollection AddFeatureStore<TFeatureStore>(this IServiceCollection services)
        where TFeatureStore : class, IFeatureStore
    {
        return services.AddFeatureStore(typeof(TFeatureStore));
    }

    /// <summary>
    /// Adds scoped <see cref="FeatureManager"/> and other required feature management services.
    /// </summary>
    /// <param name="services">The service collection that feature management services are added to.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="FeatureManagementException">Thrown if <see cref="FeatureManager"/> has been registered as singleton or <see cref="IFeatureStore"/> was not registered.</exception>
    public static IFeatureManagementBuilder AddDatabaseFeatureManagement(this IServiceCollection services)
    {
        if (!services.Any(descriptor => descriptor.ServiceType == typeof(IFeatureStore)))
            throw new FeatureManagementException(FeatureManagementError.Conflict, "You need to register the IFeatureStore service.");

        services.AddDatabaseFeatureDefinitionProvider();

        return services.AddScopedFeatureManagement();
    }

    /// <summary>
    /// Adds scoped <see cref="FeatureManager"/> and other required feature management services.
    /// </summary>
    /// <typeparam name="TFeatureStore">The service type used to retrieve data from database.</typeparam>
    /// <param name="services">The service collection that feature management services are added to.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="FeatureManagementException">Thrown if <see cref="FeatureManager"/> has been registered as singleton or <see cref="IFeatureStore"/> was already registered.</exception>
    public static IFeatureManagementBuilder AddDatabaseFeatureManagement<TFeatureStore>(this IServiceCollection services)
        where TFeatureStore : class, IFeatureStore
    {
        services.AddFeatureStore<TFeatureStore>();
        services.AddDatabaseFeatureDefinitionProvider();

        return services.AddScopedFeatureManagement();
    }

    #region Internal

    /// <summary>
    /// Adds a service built on top the <see cref="IFeatureStore"/> to the service collection in order
    /// to manage database feature management cache.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="cacheConfiguration">
    /// The <see cref="IConfiguration"/> for database feature management cache options.
    /// If <see langword="null"/> (default value), pre-configured option will be used.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    internal static IServiceCollection ConfigureCachedFeatureStore(this IServiceCollection services, IConfiguration cacheConfiguration = null)
    {
        if (cacheConfiguration is null)
            return services.ConfigureCachedFeatureStore(_ => new FeatureCacheOptions());

        services.Configure<FeatureCacheOptions>(cacheConfiguration);
        return services.AddDatabaseFeatureManagementCacheServices();
    }

    /// <summary>
    /// Adds a service built on top the <see cref="IFeatureStore"/> to the service collection in order
    /// to manage database feature management cache.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configureCacheOptions">An action used to configure database feature management cache options.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    internal static IServiceCollection ConfigureCachedFeatureStore(this IServiceCollection services, Action<FeatureCacheOptions> configureCacheOptions)
    {
        services.Configure(configureCacheOptions);
        return services.AddDatabaseFeatureManagementCacheServices();
    }

    #endregion Internal

    #region Private

    private static void AddDatabaseFeatureDefinitionProvider(this IServiceCollection services)
    {
        services.AddScoped<IFeatureDefinitionProvider, DatabaseFeatureDefinitionProvider>();
    }

    private static IServiceCollection AddDatabaseFeatureManagementCacheServices(this IServiceCollection services)
    {
#if NET9_0_OR_GREATER
        services.AddHybridCache().AddSerializer<Feature, FeatureHybridCacheSerializer>();
#else
        services.AddDistributedMemoryCache();
#endif
        return services.Decorate<IFeatureStore, CachedFeatureStore>();
    }

    #endregion Private
}