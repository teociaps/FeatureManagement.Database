// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureManagement.Database;

/// <summary>
/// Extension methods used to add database feature management functionality.
/// </summary>
public static class FeatureManagementBuilderExtensions
{
    /// <summary>
    /// Configures the feature management system to use Dapper.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IFeatureManagementBuilder"/> used to customize feature management functionality.
    /// </param>
    /// <param name="dbConnectionFactory">The factory used to configure the database connection.</param>
    /// <returns>
    /// A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.
    /// </returns>
    public static IFeatureManagementBuilder UseDapper(
        this IFeatureManagementBuilder builder,
        IDbConnectionFactory dbConnectionFactory)
    {
        if (dbConnectionFactory is null)
            throw new ArgumentNullException(nameof(dbConnectionFactory));

        builder.Services.AddTransient(_ => dbConnectionFactory);

        return builder;
    }
}