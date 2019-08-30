using System;
using System.Runtime.CompilerServices;

namespace Velo.Dependencies
{
    public sealed class DependencyResolver
    {
        private readonly string _dependencyName;
        private readonly IDependency _dependency;
        private readonly bool _scopeDependency;

        private bool _alreadyInScope;
        private bool _resolveInProgress;

        internal DependencyResolver(IDependency dependency, string dependencyName = null, bool scopeDependency = false)
        {
            _dependencyName = dependencyName;
            _dependency = dependency;
            _scopeDependency = scopeDependency;
        }

        public bool Applicable(Type requestedType, string parameterName = null)
        {
            return _dependencyName == null || parameterName == null
                ? _dependency.Applicable(requestedType)
                : _dependencyName == parameterName && _dependency.Applicable(requestedType); 
        }

        public void Destroy()
        {
            _dependency.Destroy();
            _alreadyInScope = false;
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            StartResolving();

            var resolved = _dependency.Resolve(requestedType, container);

            ResolvingComplete();

            return resolved;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartResolving()
        {
            if (_resolveInProgress)
            {
                throw new InvalidOperationException($"Detected circular dependency {_dependency}");
            }

            _resolveInProgress = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResolvingComplete()
        {
            _resolveInProgress = false;

            if (!_scopeDependency || _alreadyInScope) return;

            DependencyScope.Register(this);
            _alreadyInScope = true;
        }
        
        public override string ToString()
        {
            return _dependency.ToString();
        }
    }
}