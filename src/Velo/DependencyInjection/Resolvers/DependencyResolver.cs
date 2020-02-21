using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    public abstract class DependencyResolver
    {
        public static DependencyResolver Build(DependencyLifetime lifetime, Type implementation, IDependencyEngine engine)
        {
            switch (lifetime)
            {
                case DependencyLifetime.Scoped:
                    return new CompiledResolver(implementation, engine);
                case DependencyLifetime.Singleton:
                    return new ActivatorResolver(implementation);
                case DependencyLifetime.Transient:
                    return new CompiledResolver(implementation, engine);
                default:
                    throw Error.InvalidDependencyLifetime();
            }
        }
        
        public Type Implementation { get; }
        
        private bool _resolveInProgress;

        protected DependencyResolver(Type implementation)
        {
            Implementation = implementation;
        }

        public object Resolve(Type contract, IDependencyScope scope)
        {
            if (_resolveInProgress) throw Error.CircularDependency(contract);

            _resolveInProgress = true;

            var instance = ResolveInstance(contract, scope);

            _resolveInProgress = false;

            return instance;
        }

        protected abstract object ResolveInstance(Type contract, IDependencyScope scope);
    }
}