using System;
using Velo.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyProvider(this IServiceCollection services,
            Action<DependencyCollection> dependencies)
        {
            var dependencyCollection = new DependencyCollection();
            dependencies(dependencyCollection);

            services.AddSingleton(dependencyCollection.BuildProvider());
            return services;
        }

        public static DependencyProvider BuildDependencyProvider(this IServiceCollection services)
        {
            var dependencies = new DependencyCollection();

            dependencies.AddServiceCollection(services);

            return dependencies.BuildProvider();
        }

        public static bool RemoveLessLifetimeService(this IServiceCollection services, Type serviceType,
            ServiceLifetime lifetime, bool notFoundResult = false)
        {
            foreach (var service in services)
            {
                if (service.ServiceType != serviceType) continue;
                return service.Lifetime < lifetime && services.Remove(service);
            }

            return notFoundResult;
        }
    }
}