using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    public abstract class DependencyResolver
    {
        public static DependencyResolver Build(DependencyLifetime lifetime, Type implementation)
        {
            switch (lifetime)
            {
                case DependencyLifetime.Scoped:
                case DependencyLifetime.Transient:
                    return new CompiledResolver(implementation);
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

        public abstract void Init(DependencyLifetime lifetime, IDependencyEngine engine);

        protected void EnsureValidDependenciesLifetime(
            ConstructorInfo constructor,
            DependencyLifetime dependedLifetime, IDependencyEngine engine)
        {
            var parameters = constructor.GetParameters();

            foreach (var parameter in parameters)
            {
                var dependencyType = parameter.ParameterType;
                var dependency = parameter.IsOptional
                    ? engine.GetDependency(dependencyType)
                    : engine.GetRequiredDependency(dependencyType);

                if (dependency == null) continue;

                var dependencyLifetime = dependency.Lifetime;
                if (dependedLifetime == DependencyLifetime.Singleton && dependencyLifetime == DependencyLifetime.Scoped)
                {
                    throw Error.InconsistentLifetime(Implementation, dependedLifetime, dependencyType, dependencyLifetime);
                }
            }
        }

        protected abstract object ResolveInstance(Type contract, IServiceProvider services);
    }
}