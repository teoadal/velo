using Microsoft.Extensions.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class DependencyCollectionExtensions
    {
        public static DependencyCollection AddServiceCollection(this DependencyCollection dependencies, IServiceCollection services)
        {
            foreach (var descriptor in services)
            {
                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        AddSingleton(dependencies, descriptor);
                        break;
                    case ServiceLifetime.Scoped:
                        AddScoped(dependencies, descriptor);
                        break;
                    case ServiceLifetime.Transient:
                        AddTransient(dependencies, descriptor);
                        break;
                    default:
                        throw Error.OutOfRange($"Unsupported dependency lifetime {descriptor.Lifetime}");
                }
            }

            return dependencies;
        }

        private static void AddSingleton(DependencyCollection dependencies, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                dependencies.AddInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
            }
            else if (descriptor.ImplementationFactory == null)
            {
                dependencies.AddSingleton(descriptor.ServiceType, descriptor.ImplementationType);
            }
            else
            {
                var contracts = new[] {descriptor.ServiceType};
                var resolver = BuildDelegateResolver(descriptor);

                dependencies.Add(new SingletonDependency(contracts, resolver));
            }
        }

        private static void AddScoped(DependencyCollection dependencies, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationFactory == null)
            {
                dependencies.AddScoped(descriptor.ServiceType, descriptor.ImplementationType);
            }
            else
            {
                var contracts = new[] {descriptor.ServiceType};
                var resolver = BuildDelegateResolver(descriptor);

                dependencies.Add(new ScopedDependency(contracts, resolver));
            }
        }

        private static void AddTransient(DependencyCollection dependencies, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationFactory == null)
            {
                dependencies.AddTransient(descriptor.ServiceType, descriptor.ImplementationType);
            }
            else
            {
                var contracts = new[] {descriptor.ServiceType};
                var resolver = BuildDelegateResolver(descriptor);

                dependencies.Add(new TransientDependency(contracts, resolver));
            }
        }

        private static DependencyResolver BuildDelegateResolver(ServiceDescriptor descriptor)
        { 
            var implementation = descriptor.ServiceType ?? descriptor.ImplementationType;
            return new DelegateResolver(implementation, descriptor.ImplementationFactory);
        }
    }
}