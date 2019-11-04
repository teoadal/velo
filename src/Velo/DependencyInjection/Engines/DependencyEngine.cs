using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Engines
{
    public abstract class DependencyEngine : IDisposable
    {
        protected readonly Dictionary<Type, Dependency> Collection;

        private bool _disposed;
        private readonly ResolverFactory[] _factories;

        internal DependencyEngine(Dictionary<Type, Dependency> collection, ResolverFactory[] factories)
        {
            Collection = collection ?? new Dictionary<Type, Dependency>();
            _factories = factories;
        }

        internal DependencyEngine(int capacity, ResolverFactory[] factories)
        {
            Collection = new Dictionary<Type, Dependency>(capacity);
            _factories = factories;
        }

        public Dependency GetDependency(Type contract, bool throwIfNotRegistered = true)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyEngine));

            if (Collection.TryGetValue(contract, out var existsDependency))
            {
                return existsDependency;
            }

            var foundDependency = FindDependency(contract);

            var factories = _factories;
            for (var i = 0; i < factories.Length; i++)
            {
                var factory = factories[i];
                if (!factory.Applicable(contract)) continue;
                return InsertDependency(contract, factory.GetResolver(contract));
            }

            if (throwIfNotRegistered && foundDependency == null)
            {
                throw Error.NotFound($"Dependency with contract {ReflectionUtils.GetName(contract)} is not registered");
            }

            return foundDependency;
        }

        protected Dependency CreateDependency(Type contract, DependencyResolver resolver)
        {
            resolver.Init(this);

            switch (resolver.Lifetime)
            {
                case DependencyLifetime.Scope:
                    return new ScopeDependency(resolver);
                case DependencyLifetime.Singleton:
                    return new SingletonDependency(resolver);
                case DependencyLifetime.Transient:
                    return new TransientDependency(resolver);
                default:
                    throw Error.InvalidData(
                        $"Invalid lifetime configuration of dependency '{ReflectionUtils.GetName(contract)}'");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Dependency InsertDependency(Type contract, DependencyResolver resolver)
        {
            var dependency = CreateDependency(contract, resolver);

            Collection.Add(contract, dependency);

            return dependency;
        }

        protected abstract Dependency FindDependency(Type contract);

        public virtual void Dispose()
        {
            _disposed = true;
        }
    }
}