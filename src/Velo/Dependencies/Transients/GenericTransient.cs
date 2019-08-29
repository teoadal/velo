using System;

namespace Velo.Dependencies.Transients
{
    internal sealed class GenericTransient : IDependency
    {
        private readonly Type _genericType;

        public GenericTransient(Type genericType)
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