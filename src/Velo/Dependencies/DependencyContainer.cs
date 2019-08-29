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

                resolvedParameters[i] = Resolve(parameter.ParameterType, parameter.Name, required);
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

        public TContract Resolve<TContract>(string name = null) where TContract : class
        {
            return (TContract) Resolve(Typeof<TContract>.Raw, name);
        }

        public object Resolve(Type contract, string name = null, bool throwInNotExists = true)
        {
            var description = new ResolverDescription(contract, name);
            if (_concreteResolvers.TryGetValue(description, out var resolver))
            {
                return resolver.Resolve(contract, this);
            }

            var resolvers = _resolvers;
            for (var i = 0; i < resolvers.Length; i++)
            {
                resolver = resolvers[i];
                if (!resolver.Applicable(contract, name)) continue;

                _concreteResolvers.Add(description, resolver);

                return resolver.Resolve(contract, this);
            }

            if (throwInNotExists)
            {
                throw new InvalidOperationException($"Dependency for contract '{contract}' is not registered");
            }

            return null;
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