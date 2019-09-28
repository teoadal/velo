using System;

namespace Velo.Dependencies.Resolvers
{
    internal abstract class DependencyResolver : IDependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly string _dependencyName;

        protected DependencyResolver(IDependency dependency, string dependencyName)
        {
            _dependency = dependency;
            _dependencyName = dependencyName;
        }

        public bool Applicable(Type contract, string parameterName = null)
        {
            return _dependencyName == null || parameterName == null
                ? _dependency.Applicable(contract)
                : _dependencyName == parameterName && _dependency.Applicable(contract);
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
        
        public override string ToString()
        {
            return $"{GetType().Name} for {_dependency}";
        }
    }
}