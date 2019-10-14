using System;

namespace Velo.DependencyInjection.Scan
{
    internal sealed class AssignableAllover : DependencyAllover
    {
        private readonly Type _contract;
        private readonly DependencyLifetime _lifetime;

        public AssignableAllover(Type contract, DependencyLifetime lifetime)
        {
            _contract = contract;
            _lifetime = lifetime;
        }

        public override void TryRegister(DependencyCollection collection, Type implementation)
        {
            if (!_contract.IsAssignableFrom(implementation)) return;

            collection.Add(_contract, implementation, _lifetime);
        }
    }
}