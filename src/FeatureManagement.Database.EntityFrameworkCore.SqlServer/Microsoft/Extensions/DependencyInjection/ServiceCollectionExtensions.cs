// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring DbContext for SQL Server database.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context which inherits from <see cref="FeatureManagementDbContext"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="dbContextOptionsBuilder">An optional action used to configure the <see cref="FeatureManagementDbContext"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddFeatureManagementDbContext<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsBuilder = null)
        where TDbContext : FeatureManagementDbContext
    {
        dbContextOptionsBuilder += x => x.UseSqlServer(ComposeDefaultSqlServerOptionsBuilder());

        return services.AddDbContext<TDbContext>(dbContextOptionsBuilder);
    }

    /// <summary>
    /// Registers the given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context which inherits from <see cref="FeatureManagementDbContext"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="connectionString">The connection string used to configure the connection of context to SQL Server database.</param>
    /// <param name="sqlServerDbContextOptionsBuilder">An optional action used to configure the SQL Server connection.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddFeatureManagementDbContext<TDbContext>(
        this IServiceCollection services,
        string connectionString,
        Action<SqlServerDbContextOptionsBuilder> sqlServerDbContextOptionsBuilder = null)
        where TDbContext : FeatureManagementDbContext
    {
        sqlServerDbContextOptionsBuilder += ComposeDefaultSqlServerOptionsBuilder();

        return services.AddDbContext<TDbContext>(options => options.UseSqlServer(connectionString, sqlServerDbContextOptionsBuilder));
    }

    private static Action<SqlServerDbContextOptionsBuilder> ComposeDefaultSqlServerOptionsBuilder()
    {
        return x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }
}