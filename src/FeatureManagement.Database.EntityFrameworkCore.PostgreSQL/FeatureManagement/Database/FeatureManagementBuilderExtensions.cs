// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

// Ignore Spelling: Npgsql

using FeatureManagement.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use PostgreSQL as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseNpgsql{TDbContext}(IFeatureManagementBuilder, Action{NpgsqlDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="npgsqlDbContextOptionsBuilder">
    /// An optional action used to configure the PostgreSQL connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseNpgsql(
        this IFeatureManagementBuilder builder,
        Action<NpgsqlDbContextOptionsBuilder> npgsqlDbContextOptionsBuilder = null)
    {
        return builder.UseNpgsql<FeatureManagementDbContext>(npgsqlDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use PostgreSQL as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseNpgsql{TDbContext}(IFeatureManagementBuilder, string, Action{NpgsqlDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to PostgreSQL database.</param>
    /// <param name="npgsqlDbContextOptionsBuilder">
    /// An optional action used to configure the PostgreSQL connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseNpgsql(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<NpgsqlDbContextOptionsBuilder> npgsqlDbContextOptionsBuilder = null)
    {
        return builder.UseNpgsql<FeatureManagementDbContext>(connectionString, npgsqlDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use PostgreSQL as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="npgsqlDbContextOptionsBuilder">
    /// An optional action used to configure the PostgreSQL connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseNpgsql<TDbContext>(
        this IFeatureManagementBuilder builder,
        Action<NpgsqlDbContextOptionsBuilder> npgsqlDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        npgsqlDbContextOptionsBuilder += ComposeDefaultSqlServerOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(builder => builder.UseNpgsql(npgsqlDbContextOptionsBuilder));
        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use PostgreSQL as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to PostgreSQL database.</param>
    /// <param name="npgsqlDbContextOptionsBuilder">
    /// An optional action used to configure the PostgreSQL connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseNpgsql<TDbContext>(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<NpgsqlDbContextOptionsBuilder> npgsqlDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        npgsqlDbContextOptionsBuilder += ComposeDefaultSqlServerOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(builder => builder.UseNpgsql(connectionString, npgsqlDbContextOptionsBuilder));

        return builder;
    }

    private static Action<NpgsqlDbContextOptionsBuilder> ComposeDefaultSqlServerOptionsBuilder()
    {
        return options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }
}