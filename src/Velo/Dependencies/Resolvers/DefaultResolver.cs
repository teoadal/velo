using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class DefaultResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly object _lockObject;
        
        private bool _resolveInProgress;

        public DefaultResolver(IDependency dependency, string dependencyName = null)
            : base(dependency, dependencyName)
        {
            _dependency = dependency;
            _lockObject = new object();
        }
        
        public override object Resolve(Type contract, DependencyContainer container)
        {
            using (Lock.Enter(_lockObject))
            {
                if (_resolveInProgress) throw Error.CircularDependency(_dependency);
                
                _resolveInProgress = true;
                var resolved = _dependency.Resolve(contract, container);
                _resolveInProgress = false;

                return resolved;
            }
        }
    }
}