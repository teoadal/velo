using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.DependencyInjection;

namespace Velo.Extensions.DependencyInjection.Scope
{
    internal sealed class ServiceScope : IServiceScope, IServiceScopeFactory, IServiceProvider
    {
        public IServiceProvider ServiceProvider => _scope;

        private readonly IDependencyScope _scope;

        public ServiceScope(IDependencyScope scope)
        {
            _scope = scope;
        }

        public IServiceScope CreateScope()
        {
            return new ServiceScope(_scope.StartScope());
        }

        public object? GetService(Type serviceType)
        {
            return _scope.GetService(serviceType);
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}