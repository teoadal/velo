using System;

namespace Velo.Dependencies.Transients
{
    internal sealed class GenericTransient : GenericDependency
    {
        private readonly Type _genericImplementation;

        public GenericTransient(Type[] genericContracts, Type genericImplementation = null) : base(genericContracts)
        {
            _genericImplementation = genericImplementation;
        }

        public override void Destroy()
        {
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            var implementation = _genericImplementation == null
                ? contract
                : _genericImplementation.MakeGenericType(contract.GetGenericArguments());

            return container.Activate(implementation);
        }
    }
}