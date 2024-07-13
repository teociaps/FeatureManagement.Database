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
    /// Configures the feature management system to use MySql as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseMySql{TDbContext}(IFeatureManagementBuilder, ServerVersion, Action{MySqlDbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="serverVersion">The server version.</param>
    /// <param name="mySqlDbContextOptionsBuilder">
    /// An optional action used to configure the MySql connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseMySql(
        this IFeatureManagementBuilder builder,
        ServerVersion serverVersion,
        Action<MySqlDbContextOptionsBuilder> mySqlDbContextOptionsBuilder = null)
    {
        return builder.UseMySql<FeatureManagementDbContext>(serverVersion, mySqlDbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use MySql as the data store.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="UseMySql{TDbContext}(IFeatureManagementBuilder, string, Action{MySqlDbContextOptionsBuilder}, ServerVersion)"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to MySql database.</param>
    /// <param name="mySqlDbContextOptionsBuilder">
    /// An optional action used to configure the MySql connection.
    /// </param>
    /// <param name="serverVersion">
    /// The server version. If <see langword="null"/>, it will be retrieved from the specified connection string.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseMySql(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder> mySqlDbContextOptionsBuilder = null,
        ServerVersion serverVersion = null)
    {
        return builder.UseMySql<FeatureManagementDbContext>(connectionString, mySqlDbContextOptionsBuilder, serverVersion);
    }

    /// <summary>
    /// Configures the feature management system to use MySql as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="serverVersion">The server version.</param>
    /// <param name="mySqlDbContextOptionsBuilder">
    /// An optional action used to configure the MySql connection.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseMySql<TDbContext>(
        this IFeatureManagementBuilder builder,
        ServerVersion serverVersion = null,
        Action<MySqlDbContextOptionsBuilder> mySqlDbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        mySqlDbContextOptionsBuilder += ComposeDefaultMySqlOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(dbContextBuilder =>
            dbContextBuilder.UseMySql(SetServerVersion(serverVersion, null), mySqlDbContextOptionsBuilder));

        return builder;
    }

    /// <summary>
    /// Configures the feature management system to use MySql as the data store.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="connectionString">The connection string used to configure the connection of context to MySql database.</param>
    /// <param name="mySqlDbContextOptionsBuilder">
    /// An optional action used to configure the MySql connection.
    /// </param>
    /// <param name="serverVersion">
    /// The server version. If <see langword="null"/>, it will be retrieved from the specified connection string.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if provided connection string is null or empty.</exception>
    public static IFeatureManagementBuilder UseMySql<TDbContext>(
        this IFeatureManagementBuilder builder,
        string connectionString,
        Action<MySqlDbContextOptionsBuilder> mySqlDbContextOptionsBuilder = null,
        ServerVersion serverVersion = null)
            where TDbContext : FeatureManagementDbContext
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Use a valid value for connection string.", nameof(connectionString));

        mySqlDbContextOptionsBuilder += ComposeDefaultMySqlOptionsBuilder();

        builder.ConfigureDbContext<TDbContext>(dbContextBuilder =>
            dbContextBuilder.UseMySql(connectionString, SetServerVersion(serverVersion, connectionString), mySqlDbContextOptionsBuilder));

        return builder;
    }

    private static Action<MySqlDbContextOptionsBuilder> ComposeDefaultMySqlOptionsBuilder()
    {
        return options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }

    private static ServerVersion SetServerVersion(ServerVersion serverVersion, string connectionString)
    {
        return serverVersion ?? ServerVersion.AutoDetect(connectionString);
    }
}