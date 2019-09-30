using System;
using System.Linq;

namespace Velo.Dependencies
{
    internal abstract class GenericDependency : IDependency
    {
        private readonly Type[] _genericContracts;

        protected GenericDependency(Type[] genericContracts)
        {
            _genericContracts = genericContracts;
        }

        public bool Applicable(Type contract)
        {
            if (!contract.IsGenericType) return false;

            var contractGenericDefinition = contract.GetGenericTypeDefinition();

            var contracts = _genericContracts;
            for (var i = 0; i < contracts.Length; i++)
            {
                if (contracts[i] == contractGenericDefinition)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Destroy()
        {
        }

        public virtual void Init(DependencyContainer container)
        {
        }

        public abstract object Resolve(Type contract, DependencyContainer container);

        public override string ToString()
        {
            var contractNames = string.Join(",", _genericContracts.Select(c => c.Name));
            return $"{contractNames} ({GetType().Name})";
        }
    }
}