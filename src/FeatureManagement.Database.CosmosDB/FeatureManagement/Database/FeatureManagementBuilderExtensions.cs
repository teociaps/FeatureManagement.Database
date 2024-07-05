// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.CosmosDB;
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
    /// Configures the feature management system to use CosmosDB.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Action<CosmosDBOptions> configureOptions)
    {
        return UseCosmosDB<CosmosDBConnectionFactory>(builder, configureOptions);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="configuration">The configuration to bind the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        IConfiguration configuration)
    {
        return UseCosmosDB<CosmosDBConnectionFactory>(builder, configuration);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="cosmosDBConnectionFactoryType">The type of the CosmosDB connection factory.</param>
    /// <param name="configuration">The configuration to bind the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the provided factory type is not <see cref="ICosmosDBConnectionFactory"/>.</exception>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Type cosmosDBConnectionFactoryType,
        IConfiguration configuration)
    {
        if (!typeof(ICosmosDBConnectionFactory).IsAssignableFrom(cosmosDBConnectionFactoryType))
        {
            throw new ArgumentException(
                "The provided factory type must inherit from ICosmosDBConnectionFactory.",
                nameof(cosmosDBConnectionFactoryType));
        }

        builder.Services.AddOptions();
        builder.Services.Configure<CosmosDBOptions>(configuration);
        builder.Services.AddSingleton(typeof(ICosmosDBConnectionFactory), cosmosDBConnectionFactoryType);

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="cosmosDBConnectionFactoryType">The type of the CosmosDB connection factory.</param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the provided factory type is not <see cref="ICosmosDBConnectionFactory"/>.</exception>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Type cosmosDBConnectionFactoryType,
        Action<CosmosDBOptions> configureOptions)
    {
        if (!typeof(ICosmosDBConnectionFactory).IsAssignableFrom(cosmosDBConnectionFactoryType))
        {
            throw new ArgumentException(
                "The provided factory type must inherit from ICosmosDBConnectionFactory.",
                nameof(cosmosDBConnectionFactoryType));
        }

        builder.Services.AddOptions();
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton(typeof(ICosmosDBConnectionFactory), cosmosDBConnectionFactoryType);

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <typeparam name="TConnectionFactory">The type of the CosmosDB connection factory.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="configuration">The configuration to bind the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseCosmosDB<TConnectionFactory>(
        this IFeatureManagementBuilder builder,
        IConfiguration configuration)
        where TConnectionFactory : class, ICosmosDBConnectionFactory
    {
        return UseCosmosDB(builder, typeof(TConnectionFactory), configuration);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a specified connection factory type.
    /// </summary>
    /// <typeparam name="TConnectionFactory">The type of the CosmosDB connection factory.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseCosmosDB<TConnectionFactory>(
        this IFeatureManagementBuilder builder,
        Action<CosmosDBOptions> configureOptions)
        where TConnectionFactory : class, ICosmosDBConnectionFactory
    {
        return UseCosmosDB(builder, typeof(TConnectionFactory), configureOptions);
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a custom connection factory.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="cosmosDBConnectionFactory">The CosmosDB connection factory.</param>
    /// <param name="configureOptions">A delegate to configure the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        ICosmosDBConnectionFactory cosmosDBConnectionFactory,
        Action<CosmosDBOptions> configureOptions)
    {
        builder.Services.AddOptions();
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton(cosmosDBConnectionFactory);

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB with a custom connection factory.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="cosmosDBConnectionFactory">The CosmosDB connection factory.</param>
    /// <param name="configuration">The configuration to bind the CosmosDB options.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        ICosmosDBConnectionFactory cosmosDBConnectionFactory,
        IConfiguration configuration)
    {
        builder.Services.AddOptions();
        builder.Services.Configure<CosmosDBOptions>(configuration);
        builder.Services.AddSingleton(cosmosDBConnectionFactory);

        return builder;
    }
}