using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeResolver : IDependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly string _dependencyName;

        private bool _addedToScope;
        private bool _resolveInProgress;

        public ScopeResolver(IDependency dependency, string name = null)
        {
            _dependency = dependency;
            _dependencyName = name;
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
            _addedToScope = false;
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            if (_resolveInProgress) throw Error.CircularDependency(_dependency);

            _resolveInProgress = true;
            
            var resolved = _dependency.Resolve(contract, container);
            
            if (!_addedToScope)
            {
                DependencyScope.Register(this);
                _addedToScope = true;
            }
                
            _resolveInProgress = false;
            
            return resolved;
        }

        public override string ToString()
        {
            return _dependency.ToString();
        }
    }
}