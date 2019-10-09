using System;

namespace Velo.Dependencies.Resolvers
{
    internal abstract class DependencyResolver : IDependency
    {
        private readonly IDependency _dependency;

        protected DependencyResolver(IDependency dependency)
        {
            _dependency = dependency;
        }

        public bool Applicable(Type contract)
        {
            return _dependency.Applicable(contract);
        }

        public void Init(DependencyContainer container)
        {
            _dependency.Init(container);
        }

        public void Destroy()
        {
            _dependency.Destroy();
        }

        public abstract object Resolve(Type contract, DependencyContainer container);
    }
}