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
        builder.Services.AddOptions();
        builder.Services.Configure(configureOptions);
        builder.Services.AddSingleton<ICosmosDBConnectionFactory, CosmosDBConnectionFactory>();

        return builder;
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
        builder.Services.AddOptions();
        builder.Services.Configure<CosmosDBOptions>(configuration);
        builder.Services.AddSingleton<ICosmosDBConnectionFactory, CosmosDBConnectionFactory>();

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB.
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
        builder.Services.AddSingleton(_ => cosmosDBConnectionFactory);

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use CosmosDB.
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
        builder.Services.AddSingleton(_ => cosmosDBConnectionFactory);

        return builder;
    }
}