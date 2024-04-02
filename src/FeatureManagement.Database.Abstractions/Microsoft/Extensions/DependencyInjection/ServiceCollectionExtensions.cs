using FeatureManagement.Database.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring feature store in ASP.NET Core applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <paramref name="implementation"/> of <see cref="IFeatureStore"/> to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="implementation">The implementation of <see cref="IFeatureStore"/> to register.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddFeatureStore(this IServiceCollection services, IFeatureStore implementation)
    {
        return services.AddScoped(typeof(IFeatureStore), implementation?.GetType() ?? typeof(NullFeatureStore));
    }
}
