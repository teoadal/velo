using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;

namespace Velo.Extensions.DependencyInjection.Scope
{
    internal sealed class ServiceScopeDependency : IDependency
    {
        public Type[] Contracts => _contracts ??= new[] {_scopeFactoryType};

        public Type Implementation { get; }

        public DependencyLifetime Lifetime { get; }

        private Type[]? _contracts;
        private DependencyProvider? _dependencyProvider;
        private readonly Type _scopeFactoryType;

        public ServiceScopeDependency()
        {
            Implementation = typeof(ServiceScope);
            Lifetime = DependencyLifetime.Transient;

            _scopeFactoryType = typeof(IServiceScopeFactory);
        }

        public bool Applicable(Type contract)
        {
            return contract == _scopeFactoryType;
        }

        public void Init(IDependencyEngine engine)
        {
        }

        public object GetInstance(Type contract, IServiceProvider services)
        {
            _dependencyProvider ??= services.GetRequired<DependencyProvider>();
            return new ServiceScope(_dependencyProvider.StartScope());
        }

        public void Dispose()
        {
            _contracts = null;
            _dependencyProvider = null;
        }
    }
}