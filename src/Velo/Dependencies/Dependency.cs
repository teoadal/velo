using System;

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
                var existsContract = contracts[i];
                if (existsContract == contract || contract.IsAssignableFrom(existsContract))
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
    }
}