using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Dependencies.Resolvers;
using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyContainer
    {
        private static readonly Type ResolverType = typeof(IDependencyResolver);
        private static readonly MethodInfo ResolveMethod = ResolverType.GetMethod(nameof(IDependencyResolver.Resolve));
        
        private readonly Dictionary<ResolverDescription, IDependencyResolver> _concreteResolvers;
        private readonly IDependencyResolver[] _resolvers;

        internal DependencyContainer(List<IDependencyResolver> resolvers)
        {
            var containerType = Typeof<DependencyContainer>.Raw;
            var containerResolver = new DefaultResolver(new InstanceSingleton(new[] {containerType}, this));
            
            resolvers.Add(containerResolver);

            _concreteResolvers = new Dictionary<ResolverDescription, IDependencyResolver>(resolvers.Count);
            _resolvers = resolvers.ToArray();
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
                var resolver = GetResolver(contract, parameter.Name, required);
                resolvedParameters[i] = resolver?.Resolve(contract, this);
            }

            return constructor.Invoke(resolvedParameters);
        }

        public Func<DependencyContainer, T> CreateActivator<T>(ConstructorInfo constructor = null)
        {
            var resultType = typeof(T);
            if (constructor == null) constructor = ReflectionUtils.GetConstructor(resultType);
                
            var containerParameter = Expression.Parameter(typeof(DependencyContainer), "container");

            var parameters = constructor.GetParameters();
            var resolvedParameters = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                var parameterType = parameter.ParameterType;
                var parameterName = parameter.Name;
                var required = !parameter.HasDefaultValue;

                var parameterResolver = this.GetResolver(parameterType, parameterName, required);
                var resolvedParameter = parameterResolver == null
                    ? (Expression) Expression.Default(parameterType)
                    : Expression.Call(Expression.Constant(parameterResolver), ResolveMethod,
                        Expression.Constant(parameterType), containerParameter);

                resolvedParameters[i] = Expression.Convert(resolvedParameter, parameterType);
            }

            Expression body = Expression.New(constructor, resolvedParameters);

            if (resultType != typeof(object))
            {
                body = Expression.Convert(body, resultType);
            }
            
            return Expression.Lambda<Func<DependencyContainer, T>>(body, containerParameter).Compile();
        }
        
        public void Destroy()
        {
            _concreteResolvers.Clear();

            foreach (var dependency in _resolvers)
            {
                dependency.Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IDependencyResolver GetResolver(Type contract, string name = null, bool throwInNotRegistered = true)
        {
            var description = new ResolverDescription(contract, name);
            if (_concreteResolvers.TryGetValue(description, out var resolver)) return resolver;

            var resolvers = _resolvers;
            for (var i = 0; i < resolvers.Length; i++)
            {
                resolver = resolvers[i];
                if (!resolver.Applicable(contract, name)) continue;

                _concreteResolvers.Add(description, resolver);

                return resolver;
            }
            
            if (throwInNotRegistered)
            {
                throw Error.InvalidOperation($"Dependency for contract '{contract}' is not registered");
            }

            return null;
        }

        public TContract Resolve<TContract>(string name = null) where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            
            var resolver = GetResolver(contract, name);
            return (TContract) resolver?.Resolve(contract, this);
        }

        public object Resolve(Type contract, string name = null, bool throwInNotRegistered = true)
        {
            var resolver = GetResolver(contract, name, throwInNotRegistered);
            return resolver?.Resolve(contract, this);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public DependencyScope StartScope([CallerMemberName] string name = "")
        {
            return new DependencyScope(name);
        }
        
        private readonly struct ResolverDescription : IEquatable<ResolverDescription>
        {
            private readonly int _hash;
            
            public ResolverDescription(Type contract, string name)
            {
                unchecked
                {
                    _hash = name?.GetHashCode() ?? 1;
                    _hash = contract.GetHashCode() ^ _hash;
                }
            }

            public override int GetHashCode() => _hash;

            public bool Equals(ResolverDescription other)
            {
                return _hash == other._hash;
            }
        }
    }
}