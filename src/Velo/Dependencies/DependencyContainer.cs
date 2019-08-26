using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyContainer
    {
        private readonly DependencyResolver[] _dependencies;
        private readonly Dictionary<Type, DependencyResolver> _concreteResolvers;

        internal DependencyContainer(List<DependencyResolver> resolvers, Dictionary<Type, DependencyResolver> concreteResolvers)
        {
            var containerType = Typeof<DependencyContainer>.Raw;
            var containerResolver = new DependencyResolver(new InstanceSingleton(new[] {containerType}, this));
            
            resolvers.Add(containerResolver);

            _dependencies = resolvers.ToArray();
            _concreteResolvers = concreteResolvers;
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
            _concreteResolvers.Clear();

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
            if (_concreteResolvers.TryGetValue(type, out var resolver))
            {
                return resolver.Resolve(type, this);
            }

            var dependencies = _dependencies;
            for (var i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                if (!dependency.Applicable(type)) continue;

                _concreteResolvers.Add(type, dependency);

                return dependency.Resolve(type, this);
            }

            if (throwInNotExists)
            {
                throw new InvalidOperationException($"Dependency with type '{type}' is not registered");
            }

            return null;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public DependencyScope StartScope([CallerMemberName] string name = "")
        {
            return new DependencyScope(name);
        }
    }
}