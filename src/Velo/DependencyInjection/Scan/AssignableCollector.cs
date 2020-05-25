using System;

namespace Velo.DependencyInjection.Scan
{
    internal sealed class AssignableCollector : IDependencyCollector
    {
        private readonly Type _contract;
        private readonly DependencyLifetime _lifetime;

        public AssignableCollector(Type contract, DependencyLifetime lifetime)
        {
            _contract = contract;
            _lifetime = lifetime;
        }

        public void TryRegister(DependencyCollection collection, Type implementation)
        {
            if (!_contract.IsAssignableFrom(implementation)) return;

            collection.AddDependency(_contract, implementation, _lifetime);
        }
    }
}