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
        private readonly Dictionary<ResolverDescription, DependencyResolver> _concreteResolvers;
        private readonly DependencyResolver[] _resolvers;

        internal DependencyContainer(List<DependencyResolver> resolvers)
        {
            var containerType = Typeof<DependencyContainer>.Raw;
            var containerResolver = new DependencyResolver(new InstanceSingleton(new[] {containerType}, this));
            
            resolvers.Add(containerResolver);

            _concreteResolvers = new Dictionary<ResolverDescription, DependencyResolver>(resolvers.Count);
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

        public void Destroy()
        {
            _concreteResolvers.Clear();

            foreach (var dependency in _resolvers)
            {
                dependency.Destroy();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyResolver GetResolver(Type contract, string name = null, bool throwInNotRegistered = true)
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
                throw new InvalidOperationException($"Dependency for contract '{contract}' is not registered");
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
            
            public ResolverDescription(Type type, string name)
            {
                unchecked
                {
                    _hash = name?.GetHashCode() ?? 1;
                    _hash = type.GetHashCode() ^ _hash;
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