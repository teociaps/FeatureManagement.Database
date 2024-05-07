using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Registers the cache service to the database feature management system using default options.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder WithCacheService(this IFeatureManagementBuilder builder)
    {
        builder.Services.AddCachedFeatureStore();
        return builder;
    }

    /// <summary>
    /// Registers the cache service to the database feature management system using the provided action.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="configureCacheOptions">An action used to configure the cache options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder WithCacheService(this IFeatureManagementBuilder builder, Action<FeatureCacheOptions> configureCacheOptions)
    {
        builder.Services.AddCachedFeatureStore(configureCacheOptions);
        return builder;
    }

    /// <summary>
    /// Registers the cache service to the database feature management system using the provided configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="cacheConfiguration">The <see cref="IConfiguration"/> used to configure the cache options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder WithCacheService(this IFeatureManagementBuilder builder, IConfiguration cacheConfiguration)
    {
        builder.Services.AddCachedFeatureStore(cacheConfiguration);
        return builder;
    }
}