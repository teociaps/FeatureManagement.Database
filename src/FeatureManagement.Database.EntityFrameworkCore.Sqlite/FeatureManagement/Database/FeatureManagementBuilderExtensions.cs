// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use Sqlite as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseSqlite{TDbContext}(IFeatureManagementBuilder, Action{SqliteDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="sqliteDbContextOptionsBuilder">
    /// An optional action used to configure the Sqlite connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlite(
        this IFeatureManagementBuilder builder,
        Action<SqliteDbContextOptionsBuilder> sqliteDbContextOptionsBuilder = null)
    {
        return builder.UseSqlite<FeatureManagementDbContext>(sqliteDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use Sqlite as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseSqlite{TDbContext}(IFeatureManagementBuilder, string, Action{SqliteDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to Sqlite database.</param>
    /// <param name="sqliteDbContextOptionsBuilder">
    /// An optional action used to configure the Sqlite connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlite(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder> sqliteDbContextOptionsBuilder = null)
    {
        return builder.UseSqlite<FeatureManagementDbContext>(connectionString, sqliteDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use Sqlite as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="sqliteDbContextOptionsBuilder">
    /// An optional action used to configure the Sqlite connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlite<TDbContext>(
        this IFeatureManagementBuilder builder,
        Action<SqliteDbContextOptionsBuilder> sqliteDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        sqliteDbContextOptionsBuilder += ComposeDefaultSqliteOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite(sqliteDbContextOptionsBuilder));
        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use Sqlite as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to Sqlite database.</param>
    /// <param name="sqliteDbContextOptionsBuilder">
    /// An optional action used to configure the Sqlite connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if provided connection string is null or empty.</exception>
    public static IFeatureManagementBuilder UseSqlite<TDbContext>(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<SqliteDbContextOptionsBuilder> sqliteDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Use a valid value for connection string.", nameof(connectionString));

        sqliteDbContextOptionsBuilder += ComposeDefaultSqliteOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite(connectionString, sqliteDbContextOptionsBuilder));

        return builder;
    }

    private static Action<SqliteDbContextOptionsBuilder> ComposeDefaultSqliteOptionsBuilder()
    {
        return options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }
}