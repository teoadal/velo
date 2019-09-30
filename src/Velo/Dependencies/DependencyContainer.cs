using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Dependencies.Resolvers;
using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyContainer : IServiceProvider
    {
        private static readonly Type DependencyType = typeof(IDependency);
        private static readonly MethodInfo ResolveMethod = DependencyType.GetMethod(nameof(IDependency.Resolve));

        private readonly ConcurrentDictionary<Type, IDependency> _concreteDependencies;
        private readonly IDependency[] _dependencies;
        private readonly Dictionary<string, IDependency> _dependencyByName;
        private readonly Func<Type, IDependency> _findDependency;

        internal DependencyContainer(List<IDependency> dependencies, Dictionary<string, IDependency> dependencyByName)
        {
            var containerResolver = new DefaultResolver(new InstanceSingleton(this));
            dependencies.Add(containerResolver);

            _concreteDependencies =
                new ConcurrentDictionary<Type, IDependency>(Environment.ProcessorCount, dependencies.Count);
            _dependencies = dependencies.ToArray();
            _dependencyByName = dependencyByName;
            _findDependency = FindDependency;
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

                var contract = parameter.ParameterType;
                var dependency = GetDependency(contract, parameter.Name, required);
                resolvedParameters[i] = dependency?.Resolve(contract, this);
            }

            return constructor.Invoke(resolvedParameters);
        }

        public Func<T> CreateActivator<T>(ConstructorInfo constructor = null)
        {
            var resultType = typeof(T);
            if (constructor == null) constructor = ReflectionUtils.GetConstructor(resultType);

            var container = Expression.Constant(this);

            var parameters = constructor.GetParameters();
            var resolvedParameters = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                var parameterType = parameter.ParameterType;
                var parameterName = parameter.Name;
                var required = !parameter.HasDefaultValue;

                var parameterDependency = GetDependency(parameterType, parameterName, required);
                var resolveCall = parameterDependency == null
                    ? (Expression) Expression.Default(parameterType)
                    : Expression.Call(Expression.Constant(parameterDependency), ResolveMethod,
                        Expression.Constant(parameterType), container);

                resolvedParameters[i] = Expression.Convert(resolveCall, parameterType);
            }

            Expression body = Expression.New(constructor, resolvedParameters);

            if (resultType != typeof(object))
            {
                body = Expression.Convert(body, resultType);
            }

            return Expression.Lambda<Func<T>>(body).Compile();
        }

        public void Destroy()
        {
            _concreteDependencies.Clear();

            foreach (var dependency in _dependencies)
            {
                dependency.Destroy();
            }
        }

        public TContract Resolve<TContract>(string name = null, bool throwInNotRegistered = true)
            where TContract : class
        {
            return (TContract) Resolve(Typeof<TContract>.Raw, name, throwInNotRegistered);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Resolve(Type contract, string name = null, bool throwInNotRegistered = true)
        {
            var dependency = GetDependency(contract, name, throwInNotRegistered);
            return dependency?.Resolve(contract, this);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public DependencyScope StartScope([CallerMemberName] string name = "")
        {
            return new DependencyScope(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IDependency GetDependency(Type contract, string name = null, bool throwInNotRegistered = true)
        {
            if (name != null && _dependencyByName.TryGetValue(name, out var dependencyWithName))
            {
                if (dependencyWithName.Applicable(contract))
                {
                    return dependencyWithName;
                }
            }

            var dependency = _concreteDependencies.GetOrAdd(contract, _findDependency);

            if (dependency == null && throwInNotRegistered)
            {
                throw Error.InvalidOperation($"Dependency for contract '{contract}' is not registered");
            }

            return dependency;
        }

        private IDependency FindDependency(Type contract)
        {
            var dependencies = _dependencies;
            for (var i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                if (dependency.Applicable(contract))
                {
                    return dependency;
                }
            }

            return null;
        }

        object IServiceProvider.GetService(Type type) => Resolve(type);
    }
}