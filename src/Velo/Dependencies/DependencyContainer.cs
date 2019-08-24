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

        private readonly Stack<Type> _resolveStack;

        internal DependencyContainer(List<IDependency> dependencies, Dictionary<Type, IDependency> resolvedDependencies)
        {
            dependencies.Add(new InstanceSingleton(new[] {Typeof<DependencyContainer>.Raw}, this));
            dependencies.Add(new ArrayFactory(dependencies));

            _dependencies = dependencies.ToArray();
            _resolvedDependencies = resolvedDependencies;
            _resolveStack = new Stack<Type>(dependencies.Count);
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
            CheckCircularDependency(type);
            RegisterDependencyCall(type);

            if (_resolvedDependencies.TryGetValue(type, out var resolver))
            {
                var resolved = resolver.Resolve(type, this);
                RegisterDependencyResolve();
                return resolved;
            }

            var dependencies = _dependencies;
            for (var i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                if (!dependency.Applicable(type)) continue;

                _resolvedDependencies.Add(type, dependency);

                var resolved = dependency.Resolve(type, this);
                RegisterDependencyResolve();
                return resolved;
            }

            if (throwInNotExists)
            {
                throw new InvalidOperationException($"Dependency with type '{type}' is not registered");
            }

            RegisterDependencyResolve();
            return null;
        }

        public DependencyScope Scope([CallerMemberName] string name = "")
        {
            return new DependencyScope(name);
        }

        private void CheckCircularDependency(Type type)
        {
            if (_resolveStack.Contains(type))
            {
                throw new InvalidOperationException($"Detected circular dependency {type}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterDependencyCall(Type type)
        {
            _resolveStack.Push(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RegisterDependencyResolve()
        {
            _resolveStack.Pop();
        }
    }
}