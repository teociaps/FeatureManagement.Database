// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.NHibernate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use NHibernate.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="connectionFactory">The factory used to configure the NHibernate session.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if provided connection string is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if provided connection factory is null.</exception>
    public static IFeatureManagementBuilder UseNHibernate(
        this IFeatureManagementBuilder builder,
        string connectionString,
        INHibernateConnectionFactory connectionFactory)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Use a valid value for connection string.", nameof(connectionString));

        if (connectionFactory is null)
            throw new ArgumentNullException(nameof(connectionFactory));

        builder.Services.AddSingleton(_ => connectionFactory.CreateSessionFactory(connectionString));

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use NHibernate.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="databaseType">The pre-configured database to use.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if provided connection string is null or empty.</exception>
    public static IFeatureManagementBuilder UseNHibernate(
        this IFeatureManagementBuilder builder,
        string connectionString,
        DatabaseType databaseType)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Use a valid value for connection string.", nameof(connectionString));

        builder.Services.AddSingleton(_ => new NHibernateConnectionFactory().CreateSessionFactory(connectionString, databaseType));

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use NHibernate.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="configureOptions">The NHibernate options configuration.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if provided option configuration is null.</exception>
    public static IFeatureManagementBuilder UseNHibernate(
        this IFeatureManagementBuilder builder,
        Action<NHibernateConfigurationOptions> configureOptions)
    {
        if (configureOptions is null)
            throw new ArgumentNullException(nameof(configureOptions));

        builder.Services.AddSingleton(_ => new NHibernateConnectionFactory().CreateSessionFactory(configureOptions));

        return builder;
    }
}