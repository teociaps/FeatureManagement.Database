// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use EF Core with a database provider.<br/>
    /// <see cref="FeatureManagementDbContext"/> will be as default type of context used for feature management.
    /// </summary>
    /// <remarks>
    /// To configure a custom DbContext see <see cref="ConfigureDbContext{TDbContext}(IFeatureManagementBuilder, Action{DbContextOptionsBuilder})"/>.
    /// </remarks>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="dbContextOptionsBuilder">
    /// An optional action used to configure the <see cref="FeatureManagementDbContext"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder ConfigureDbContext(
        this IFeatureManagementBuilder builder,
        Action<DbContextOptionsBuilder> dbContextOptionsBuilder = null)
    {
        return builder.ConfigureDbContext<FeatureManagementDbContext>(dbContextOptionsBuilder);
    }

    /// <summary>
    /// Configures the feature management system to use EF Core with a database provider.
    /// </summary>
    /// <typeparam name="TDbContext">The type of context used for feature management.</typeparam>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="dbContextOptionsBuilder">
    /// An optional action used to configure the <typeparamref name="TDbContext"/>.
    /// </param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder ConfigureDbContext<TDbContext>(
        this IFeatureManagementBuilder builder,
        Action<DbContextOptionsBuilder> dbContextOptionsBuilder = null)
            where TDbContext : FeatureManagementDbContext
    {
        builder.Services.AddDbContext<TDbContext>(dbContextOptionsBuilder);
        return builder;
    }
}