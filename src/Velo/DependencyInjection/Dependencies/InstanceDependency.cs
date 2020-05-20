using System;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class InstanceDependency : DependencyResolver, IDependency
    {
        public Type[] Contracts => _contracts;

        public DependencyLifetime Lifetime => DependencyLifetime.Singleton;

        private readonly Type[] _contracts;
        private readonly object _instance;

        public InstanceDependency(Type[] contracts, object instance) : base(instance.GetType())
        {
            _contracts = contracts;
            _instance = instance;
        }

        public bool Applicable(Type request)
        {
            if (request.IsInterface)
            {
                foreach (var contract in _contracts)
                {
                    if (request.IsAssignableFrom(contract)) return true;
                }
            }

            return Array.IndexOf(_contracts, request) != -1;
        }

        public object GetInstance(Type contract, IServiceProvider services) => _instance;

        protected override object ResolveInstance(Type contract, IServiceProvider services) => _instance;

        public void Dispose()
        {
            if (ReflectionUtils.IsDisposable(_instance, out var disposable))
            {
                disposable.Dispose();
            }
        }
    }
}