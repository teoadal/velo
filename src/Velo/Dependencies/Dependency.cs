using System;
using System.Linq;

namespace Velo.Dependencies
{
    internal abstract class Dependency : IDependency
    {
        private readonly Type[] _contracts;

        protected Dependency(Type[] contracts)
        {
            _contracts = contracts;
        }

        public bool Applicable(Type contract)
        {
            var contracts = _contracts;
            for (var i = 0; i < contracts.Length; i++)
            {
                if (contracts[i] == contract)
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
            var contractNames = string.Join(",", _contracts.Select(c => c.Name));
            return $"{contractNames} ({GetType().Name})";
        }
    }
}