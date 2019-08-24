using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Dependencies.Factories;
using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyContainer
    {
        private readonly IDependency[] _dependencies;
        private readonly Dictionary<Type, IDependency> _resolvedDependencies;

        internal DependencyContainer(List<IDependency> dependencies, Dictionary<Type, IDependency> resolvedDependencies)
        {
            dependencies.Add(new InstanceSingleton(new[] {Typeof<DependencyContainer>.Raw}, this));
            dependencies.Add(new ArrayFactory(dependencies));

            _dependencies = dependencies.ToArray();
            _resolvedDependencies = resolvedDependencies;
        }

        public T Activate<T>() where T : class
        {
            return (T) Activate(Typeof<T>.Raw);
        }

        public object Activate(Type type, ConstructorInfo constructor = null)
        {
            if (constructor == null)
            {
                constructor = ReflectionUtils.GetConstructor(type);
            }

            var parameters = constructor.GetParameters();
            var resolvedParameters = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var required = !parameter.HasDefaultValue;

                resolvedParameters[i] = Resolve(parameter.ParameterType, required);
            }

            return constructor.Invoke(resolvedParameters);
        }

        public void Destroy()
        {
            _resolvedDependencies.Clear();

            foreach (var dependency in _dependencies)
            {
                dependency.Destroy();
            }
        }

        public T Resolve<T>() where T : class
        {
            return (T) Resolve(Typeof<T>.Raw);
        }

        public object Resolve(Type type, bool throwInNotExists = true)
        {
            if (_resolvedDependencies.TryGetValue(type, out var resolver))
            {
                var resolved = resolver.Resolve(type, this);
                return resolved;
            }

            for (var i = 0; i < _dependencies.Length; i++)
            {
                var dependency = _dependencies[i];
                if (!dependency.Applicable(type)) continue;

                _resolvedDependencies.Add(type, dependency);

                var resolved = dependency.Resolve(type, this);
                return resolved;
            }

            if (throwInNotExists)
            {
                throw new InvalidOperationException($"Dependency with type '{type}' is not registered");
            }

            return null;
        }

        public DependencyScope Scope([CallerMemberName] string name = "")
        {
            return new DependencyScope(name);
        }
    }
}