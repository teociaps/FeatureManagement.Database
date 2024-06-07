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
    /// Configures the feature management system to use SQL Server as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseSqlServer{TDbContext}(IFeatureManagementBuilder, Action{SqlServerDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="sqlServerDbContextOptionsBuilder">
    /// An optional action used to configure the SQL Server connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlServer(
        this IFeatureManagementBuilder builder,
        Action<SqlServerDbContextOptionsBuilder> sqlServerDbContextOptionsBuilder = null)
    {
        return builder.UseSqlServer<FeatureManagementDbContext>(sqlServerDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use SQL Server as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseSqlServer{TDbContext}(IFeatureManagementBuilder, string, Action{SqlServerDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to SQL Server database.</param>
    /// <param name="sqlServerDbContextOptionsBuilder">
    /// An optional action used to configure the SQL Server connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlServer(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder> sqlServerDbContextOptionsBuilder = null)
    {
        return builder.UseSqlServer<FeatureManagementDbContext>(connectionString, sqlServerDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use SQL Server as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="sqlServerDbContextOptionsBuilder">
    /// An optional action used to configure the SQL Server connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlServer<TDbContext>(
        this IFeatureManagementBuilder builder,
        Action<SqlServerDbContextOptionsBuilder> sqlServerDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        sqlServerDbContextOptionsBuilder += ComposeDefaultSqlServerOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(dbContextBuilder => dbContextBuilder.UseSqlServer(sqlServerDbContextOptionsBuilder));
        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use SQL Server as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to SQL Server database.</param>
    /// <param name="sqlServerDbContextOptionsBuilder">
    /// An optional action used to configure the SQL Server connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseSqlServer<TDbContext>(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder> sqlServerDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        sqlServerDbContextOptionsBuilder += ComposeDefaultSqlServerOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(dbContextBuilder => dbContextBuilder.UseSqlServer(connectionString, sqlServerDbContextOptionsBuilder));

        return builder;
    }

    private static Action<SqlServerDbContextOptionsBuilder> ComposeDefaultSqlServerOptionsBuilder()
    {
        return options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }
}