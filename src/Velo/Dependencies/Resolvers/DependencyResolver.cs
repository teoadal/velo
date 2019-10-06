using System;
using System.Diagnostics;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    [DebuggerDisplay("{GetType().Name} for {_dependency}")]
    internal abstract class DependencyResolver : IDependency
    {
        private readonly IDependency _dependency;
        private bool _resolveInProgress;

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

        protected void BeginResolving()
        {
            if (_resolveInProgress) throw Error.CircularDependency(_dependency);
            _resolveInProgress = true;
        }

        protected void ResolvingComplete()
        {
            _resolveInProgress = false;
        }
    }
}