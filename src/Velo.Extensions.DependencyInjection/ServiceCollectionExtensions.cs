using System;
using System.Linq;
using Velo.DependencyInjection;
using Velo.Extensions.DependencyInjection.Scope;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static DependencyCollection AsDependencyCollection(this IServiceCollection services)
        {
            var dependenciesDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(DependencyCollection));
            if (dependenciesDescriptor != null)
            {
                return (DependencyCollection) dependenciesDescriptor.ImplementationInstance;
            }

            var dependencies = new DependencyCollection();
            services.Add(new ServiceDescriptor(typeof(DependencyCollection), dependencies));
            return dependencies;
        }

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

            dependencies
                .Add(new ServiceScopeDependency())
                .AddServiceCollection(services);

            return dependencies.BuildProvider();
        }

        public static bool Contains(this IServiceCollection services, Type contract)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var service in services)
            {
                if (service.ServiceType == contract) return true;
            }

            return false;
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