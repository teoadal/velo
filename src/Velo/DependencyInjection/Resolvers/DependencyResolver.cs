using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    public abstract class DependencyResolver
    {
        public static DependencyResolver Build(
            DependencyLifetime lifetime,
            Type implementation,
            IDependencyEngine engine)
        {
            switch (lifetime)
            {
                case DependencyLifetime.Scoped:
                case DependencyLifetime.Transient:
                    return new CompiledResolver(implementation, engine);
                case DependencyLifetime.Singleton:
                    return new ActivatorResolver(implementation);
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

        public object Resolve(Type contract, IServiceProvider services)
        {
            if (_resolveInProgress) throw Error.CircularDependency(contract);

            _resolveInProgress = true;

            var instance = ResolveInstance(contract, services);

            _resolveInProgress = false;

            return instance;
        }

        protected abstract object ResolveInstance(Type contract, IServiceProvider services);
    }
}