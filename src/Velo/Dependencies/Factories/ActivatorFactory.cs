using System;

namespace Velo.Dependencies.Factories
{
    internal sealed class ActivatorFactory : IDependency
    {
        private readonly Type _contract;

        public ActivatorFactory(Type contract)
        {
            _contract = contract;
        }

        public bool Applicable(Type requestedType)
        {
            return _contract == requestedType;
        }

        public void Destroy()
        {
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            return container.Activate(_contract);
        }
    }
}