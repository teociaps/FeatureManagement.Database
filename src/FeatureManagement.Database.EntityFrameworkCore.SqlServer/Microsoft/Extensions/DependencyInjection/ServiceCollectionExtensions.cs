// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring EFCore DbContext for database feature management.
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
        return services.AddDbContext<TDbContext>(dbContextOptionsBuilder);
    }
}