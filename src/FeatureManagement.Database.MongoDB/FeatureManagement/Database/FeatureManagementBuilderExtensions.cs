// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

// Ignore Spelling: Mongo

using FeatureManagement.Database.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use MongoDB.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The MongoDB connection string.</param>
    /// <param name="databaseName">The name of the database to use.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseMongoDB(
        this IFeatureManagementBuilder builder,
        string connectionString,
        string databaseName)
    {
        MongoDbConfigurator.RegisterClassMaps();

        var factory = new MongoDBConnectionFactory(connectionString, databaseName);
        builder.Services.AddTransient<IMongoDbConnectionFactory>(_ => factory);

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use MongoDB.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="mongoDbConnectionFactory">The factory used to configure the MongoDB connection.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if provided factory is null.</exception>
    public static IFeatureManagementBuilder UseMongoDB(
        this IFeatureManagementBuilder builder,
        IMongoDbConnectionFactory mongoDbConnectionFactory)
    {
        if (mongoDbConnectionFactory is null)
            throw new ArgumentNullException(nameof(mongoDbConnectionFactory));

        MongoDbConfigurator.RegisterClassMaps();

        builder.Services.AddTransient(_ => mongoDbConnectionFactory);

        return builder;
    }
}