using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.DependencyInjection;
using Velo.Extensions.DependencyInjection.Scope;

namespace Velo.Extensions.DependencyInjection
{
    public sealed class DependencyProviderFactory : IServiceProviderFactory<DependencyCollection>
    {
        public DependencyCollection CreateBuilder(IServiceCollection services)
        {
            return services
                .AsDependencyCollection()
                .Add(new ServiceScopeDependency())
                .AddServiceCollection(services);
        }

        public IServiceProvider CreateServiceProvider(DependencyCollection dependencies)
        {
            return dependencies.BuildProvider();
        }
    }
}