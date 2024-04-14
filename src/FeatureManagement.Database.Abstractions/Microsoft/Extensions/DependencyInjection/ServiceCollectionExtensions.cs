// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Abstractions;
using Microsoft.Extensions.Logging;
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
            return services.AddScoped(typeof(IFeatureStore), implementationType);

        throw new ArgumentException(
            "The provided implementation type must inherits from IFeatureStore.",
            nameof(implementationType));
    }

    /// <summary>
    /// Adds an <typeparamref name="TFeatureStoreImplementation"/> for <see cref="IFeatureStore"/> to the service collection.
    /// </summary>
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

        // services.AddDistributedMemoryCache();

        return services.AddScopedFeatureManagement();
    }

    /// <summary>
    /// Adds scoped <see cref="FeatureManager"/> and other required feature management services.
    /// </summary>
    /// <param name="services">The service collection that feature management services are added to.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="FeatureManagementException">Thrown if <see cref="FeatureManager"/> has been registered as singleton.</exception>
    public static IFeatureManagementBuilder AddDatabaseFeatureManagement<TFeatureStoreImplementation>(this IServiceCollection services)
        where TFeatureStoreImplementation : class, IFeatureStore
    {
        CheckFeatureStoreRegistration(
            services.Any(descriptor => descriptor.ServiceType == typeof(IFeatureStore)),
            errorMessage: "You already registered the IFeatureStore service.");

        services.AddFeatureStore<TFeatureStoreImplementation>();
        services.AddDatabaseFeatureDefinitionProvider();

        // services.AddDistributedMemoryCache();

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
        services.AddSingleton<IFeatureDefinitionProvider>(sp =>
        {
            var featureStore = sp.GetRequiredService<IFeatureStore>();
            return new DatabaseFeatureDefinitionProvider(featureStore)
            {
                Logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<DatabaseFeatureDefinitionProvider>()
            };
        });
    }

    #endregion Private
}