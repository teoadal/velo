using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;

namespace Velo.ECS.Systems
{
    internal sealed class SystemsCollector : IDependencyCollector
    {
        private readonly DependencyLifetime _lifetime;

        public SystemsCollector(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        public void TryRegister(DependencyCollection collection, Type implementation)
        {
            if (ECSUtils.TryGetImplementedSystemInterfaces(implementation, out var contracts))
            {
                collection.AddDependency(contracts, implementation, _lifetime);
            }
        }
    }
}