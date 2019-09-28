using System;

namespace Velo.Dependencies.Transients
{
    internal sealed class GenericTransient : IDependency
    {
        private readonly Type _genericContract;
        private readonly Type _genericImplementation;

        public GenericTransient(Type genericContract, Type genericImplementation = null)
        {
            _genericContract = genericContract;
            _genericImplementation = genericImplementation;
        }

        public bool Applicable(Type contract)
        {
            return contract.IsGenericType && contract.GetGenericTypeDefinition() == _genericContract;
        }

        public void Destroy()
        {
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            var implementation = _genericImplementation == null
                ? contract
                : _genericImplementation.MakeGenericType(contract.GetGenericArguments());
            
            return container.Activate(implementation);
        }
    }
}