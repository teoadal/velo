using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal abstract class Resolver : IDependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly string _dependencyName;

        private bool _resolveInProgress;

        protected Resolver(IDependency dependency, string dependencyName)
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

        public void Destroy()
        {
            _dependency.Destroy();
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            if (_resolveInProgress) throw Error.CircularDependency(_dependency);

            _resolveInProgress = true;

            var resolved = _dependency.Resolve(contract, container);

            ResolveComplete(resolved, container);

            _resolveInProgress = false;

            return resolved;
        }

        protected virtual void DestroyComplete()
        {
        }

        protected virtual void ResolveComplete(object resolvedInstance, DependencyContainer container)
        {
        }

        public override string ToString()
        {
            return _dependency.ToString();
        }
    }
}