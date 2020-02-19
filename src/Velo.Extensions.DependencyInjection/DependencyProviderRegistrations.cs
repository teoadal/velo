using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.DependencyInjection;

namespace Velo.Extensions.DependencyInjection
{
    public static class DependencyProviderRegistrations
    {
        public static IServiceCollection AddDependencyProvider(this IServiceCollection services, 
            Action<DependencyCollection> dependencies)
        {
            var dependencyCollection = new DependencyCollection();
            dependencies(dependencyCollection);

            services.AddSingleton(dependencyCollection.BuildProvider());
            return services;
        }
    }
}