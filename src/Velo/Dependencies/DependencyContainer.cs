using System;
using System.Collections.Generic;
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

        private readonly CircularDependencyDetector _circularDetector;

        internal DependencyContainer(List<IDependency> dependencies)
        {
            dependencies.Add(new InstanceSingleton(new[] {Typeof<DependencyContainer>.Raw}, this));
            dependencies.Add(new ArrayFactory(dependencies));

            _dependencies = dependencies.ToArray();

            _circularDetector = new CircularDependencyDetector(50);
            _resolvedDependencies = new Dictionary<Type, IDependency>(_dependencies.Length);
        }

        public T Activate<T>() where T : class
        {
            return (T) Activate(Typeof<T>.Raw);
        }

        public object Activate(Type type)
        {
            var constructor = type.GetConstructors()[0];

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
            _circularDetector.Call(type, throwInNotExists);

            if (_resolvedDependencies.TryGetValue(type, out var resolver))
            {
                var resolved = resolver.Resolve(type, this);
                _circularDetector.Resolved();
                return resolved;
            }

            for (var i = 0; i < _dependencies.Length; i++)
            {
                var dependency = _dependencies[i];
                if (!dependency.Applicable(type)) continue;

                _resolvedDependencies.Add(type, dependency);

                var resolved = dependency.Resolve(type, this);
                _circularDetector.Resolved();
                return resolved;
            }

            if (throwInNotExists)
            {
                throw new InvalidOperationException($"Dependency resolver for type '{type}' is not registered");
            }

            _circularDetector.Resolved();
            return null;
        }

        public DependencyScope Scope([CallerMemberName] string name = "")
        {
            return new DependencyScope(name);
        }
    }
}