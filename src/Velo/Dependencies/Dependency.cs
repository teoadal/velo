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

        public bool Applicable(Type requestedType)
        {
            for (int i = 0; i < _contracts.Length; i++)
            {
                if (_contracts[i] == requestedType)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Destroy()
        {
        }

        public abstract object Resolve(Type requestedType, DependencyContainer container);
    }
}