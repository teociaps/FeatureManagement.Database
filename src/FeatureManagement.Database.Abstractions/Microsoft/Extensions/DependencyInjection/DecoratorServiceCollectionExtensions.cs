// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Internal extension methods for <see cref="IServiceCollection"/> used to decorate services.
/// </summary>
internal static class DecoratorServiceCollectionExtensions
{
    /// <summary>
    /// Decorates a service of type <typeparamref name="TService"/> using the <typeparamref name="TDecorator"/> type.
    /// </summary>
    /// <typeparam name="TService">The service type to be decorated.</typeparam>
    /// <typeparam name="TDecorator">The implementation type used as decorator.</typeparam>
    /// <param name="services">THe <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    internal static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
        where TDecorator : class, TService
    {
        if (services.TryGetDescriptors(typeof(TService), out var descriptors))
            return services.DecorateDescriptors(descriptors.ToArray(), x => x.Decorate(typeof(TDecorator)));

        return services;
    }

    /// <summary>
    /// Creates a new <see cref="ServiceDescriptor"/> representing the decorator service.
    /// </summary>
    /// <param name="descriptor">The descriptor instance to decorate.</param>
    /// <param name="decoratorType">The type of decorator.</param>
    /// <returns>The <see cref="ServiceDescriptor"/> of the decorator service.</returns>
    internal static ServiceDescriptor Decorate(this ServiceDescriptor descriptor, Type decoratorType)
    {
        return ServiceDescriptor.Describe(
            serviceType: descriptor.ServiceType,
            implementationFactory: CreateDecorator,
            lifetime: descriptor.Lifetime);

        object CreateDecorator(IServiceProvider provider)
        {
            var decoratedInstance = ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType);
            var decorator = ActivatorUtilities.CreateInstance(provider, decoratorType, decoratedInstance);
            return decorator;
        }
    }

    #region Private

    /// <summary>
    /// Tries to get all service descriptors by the provided <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <param name="serviceType">The <see langword="type"/>of the service to decorate.</param>
    /// <param name="descriptors">The list of descriptors to decorate.</param>
    /// <returns><see langword="true"/> if any service can be decorated; otherwise, <see langword="false"/>.</returns>
    private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out IEnumerable<ServiceDescriptor> descriptors)
    {
        descriptors = services.Where(descriptor => descriptor.ServiceType == serviceType);
        return descriptors.Any();
    }

    /// <summary>
    /// Replaces the service descriptors with decorator service using the provided factory.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance</param>
    /// <param name="descriptors">The list of descriptors to decorate.</param>
    /// <param name="decorator">A factory used to decorate the services.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    private static IServiceCollection DecorateDescriptors(this IServiceCollection services, ServiceDescriptor[] descriptors, Func<ServiceDescriptor, ServiceDescriptor> decorator)
    {
        for (var i = descriptors.Length - 1; i >= 0; i--)
        {
            var descriptor = descriptors[i];

            services.Replace(decorator(descriptor));
        }

        return services;
    }

    #endregion Private
}