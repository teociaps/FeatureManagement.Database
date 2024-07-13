// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.CosmosDB;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use CosmosDB.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options for database feature management.</param>
    /// <param name="configureClientOptions">A delegate to configure the CosmosClient options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Action<CosmosDBOptions> configureOptions,
        Action<CosmosClientOptions> configureClientOptions = null)
    {
        return UseCosmosDB<CosmosDBConnectionFactory>(builder, configureOptions, configureClientOptions);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="configuration">The configuration to bind the CosmosDB options for database feature management.</param>
    /// <param name="configureClientOptions">A delegate to configure the CosmosClient options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        IConfiguration configuration,
        Action<CosmosClientOptions> configureClientOptions = null)
    {
        return UseCosmosDB<CosmosDBConnectionFactory>(builder, configuration, configureClientOptions);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="cosmosDBConnectionFactoryType">The type of the CosmosDB connection factory.</param>
    /// <param name="configuration">The configuration to bind the CosmosDB options for database feature management.</param>
    /// <param name="configureClientOptions">A delegate to configure the CosmosClient options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided factory type is not <see cref="ICosmosDBConnectionFactory"/>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if provided configuration is null.</exception>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Type cosmosDBConnectionFactoryType,
        IConfiguration configuration,
        Action<CosmosClientOptions> configureClientOptions = null)
    {
        if (!typeof(ICosmosDBConnectionFactory).IsAssignableFrom(cosmosDBConnectionFactoryType))
        {
            throw new ArgumentException(
                "The provided factory type must inherit from ICosmosDBConnectionFactory.",
                nameof(cosmosDBConnectionFactoryType));
        }

        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        builder.Services.AddOptions();
        builder.Services.Configure<CosmosDBOptions>(configuration);
        builder.Services.AddSingleton(typeof(ICosmosDBConnectionFactory), serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CosmosDBOptions>>();
            return Activator.CreateInstance(cosmosDBConnectionFactoryType, options, configureClientOptions);
        });

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="cosmosDBConnectionFactoryType">The type of the CosmosDB connection factory.</param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options for database feature management.</param>
    /// <param name="configureClientOptions">A delegate to configure the CosmosClient options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided factory type is not <see cref="ICosmosDBConnectionFactory"/>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if provided options configuration is null.</exception>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Type cosmosDBConnectionFactoryType,
        Action<CosmosDBOptions> configureOptions,
        Action<CosmosClientOptions> configureClientOptions = null)
    {
        if (!typeof(ICosmosDBConnectionFactory).IsAssignableFrom(cosmosDBConnectionFactoryType))
        {
            throw new ArgumentException(
                "The provided factory type must inherit from ICosmosDBConnectionFactory.",
                nameof(cosmosDBConnectionFactoryType));
        }

        if (configureOptions is null)
            throw new ArgumentNullException(nameof(configureOptions));

        builder.Services.AddOptions();
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton(typeof(ICosmosDBConnectionFactory), serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<CosmosDBOptions>>();
            return Activator.CreateInstance(cosmosDBConnectionFactoryType, options, configureClientOptions);
        });

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <typeparam name="TConnectionFactory">The type of the CosmosDB connection factory.</typeparam>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="configuration">The configuration to bind the CosmosDB options for database feature management.</param>
    /// <param name="configureClientOptions">A delegate to configure the CosmosClient options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided factory or configuration is null.</exception>
    public static IFeatureManagementBuilder UseCosmosDB<TConnectionFactory>(
        this IFeatureManagementBuilder builder,
        IConfiguration configuration,
        Action<CosmosClientOptions> configureClientOptions = null)
        where TConnectionFactory : class, ICosmosDBConnectionFactory
    {
        return UseCosmosDB(builder, typeof(TConnectionFactory), configuration, configureClientOptions);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <typeparam name="TConnectionFactory">The type of the CosmosDB connection factory.</typeparam>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options for database feature management.</param>
    /// <param name="configureClientOptions">A delegate to configure the CosmosClient options.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder UseCosmosDB<TConnectionFactory>(
        this IFeatureManagementBuilder builder,
        Action<CosmosDBOptions> configureOptions,
        Action<CosmosClientOptions> configureClientOptions = null)
        where TConnectionFactory : class, ICosmosDBConnectionFactory
    {
        return UseCosmosDB(builder, typeof(TConnectionFactory), configureOptions, configureClientOptions);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a custom connection factory.
    /// </summary>
    /// <param name="builder">The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.</param>
    /// <param name="cosmosDBConnectionFactory">The CosmosDB connection factory.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    /// <exception cref="ArgumentNullException">Thrown if provided factory or configuration is null.</exception>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        ICosmosDBConnectionFactory cosmosDBConnectionFactory)
    {
        if (cosmosDBConnectionFactory is null)
            throw new ArgumentNullException(nameof(cosmosDBConnectionFactory));

        builder.Services.AddSingleton(cosmosDBConnectionFactory);

        return builder;
    }
}