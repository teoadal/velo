using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    public abstract class DependencyResolver
    {
        private bool _resolveInProgress;

        public object Resolve(Type contract, IDependencyScope scope)
        {
            if (_resolveInProgress) throw Error.CircularDependency(contract);

            _resolveInProgress = true;

            var instance = GetInstance(contract, scope);

            _resolveInProgress = false;

            return instance;
        }

        protected abstract object GetInstance(Type contract, IDependencyScope scope);
    }
}