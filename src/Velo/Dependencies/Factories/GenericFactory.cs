using System;

namespace Velo.Dependencies.Factories
{
    internal sealed class GenericFactory : IDependency
    {
        private readonly Type _genericType;

        public GenericFactory(Type genericType)
        {
            _genericType = genericType;
        }

        public bool Applicable(Type contract)
        {
            return contract.IsGenericType && contract.GetGenericTypeDefinition() == _genericType;
        }

        public void Destroy()
        {
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            return container.Activate(contract);
        }
    }
}