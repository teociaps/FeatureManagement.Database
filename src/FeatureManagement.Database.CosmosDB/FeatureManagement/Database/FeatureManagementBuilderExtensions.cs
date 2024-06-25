// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.CosmosDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

// TODO: review and fix this extension methods

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        Action<CosmosDBOptions> configureOptions)
    {
        builder.Services.AddOptions();
        builder.Services.Configure(configureOptions);

        builder.Services.AddSingleton<ICosmosDBConnectionFactory, CosmosDBConnectionFactory>();

        return builder;
    }

    public static IFeatureManagementBuilder UseCosmosDB(
        this IFeatureManagementBuilder builder,
        IConfiguration configuration)
    {
        builder.Services.AddOptions();
        builder.Services.Configure<CosmosDBOptions>(configuration);

        builder.Services.AddSingleton<ICosmosDBConnectionFactory, CosmosDBConnectionFactory>();

        return builder;
    }

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