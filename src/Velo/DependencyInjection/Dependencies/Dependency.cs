using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    public interface IDependency : IDisposable
    {
        Type[] Contracts { get; }

        DependencyLifetime Lifetime { get; }

        DependencyResolver Resolver { get; }
        
        bool Applicable(Type contract);

        object GetInstance(Type contract, IDependencyScope scope);
    }

    public abstract class Dependency : IDependency
    {
        public static Dependency Build(DependencyLifetime lifetime, Type[] contracts, DependencyResolver resolver)
        {
            switch (lifetime)
            {
                case DependencyLifetime.Scoped:
                    return new ScopedDependency(contracts, resolver);
                case DependencyLifetime.Singleton:
                    return new SingletonDependency(contracts, resolver);
                case DependencyLifetime.Transient:
                    return new TransientDependency(contracts, resolver);
                default:
                    throw Error.InvalidDependencyLifetime();
            }
        }

        public Type[] Contracts => _contracts;

        public DependencyLifetime Lifetime { get; }

        public DependencyResolver Resolver => _resolver;

        private Type[] _contracts;
        private DependencyResolver _resolver;

        protected Dependency(Type[] contracts, DependencyResolver resolver, DependencyLifetime lifetime)
        {
            Lifetime = lifetime;
            _contracts = contracts;
            _resolver = resolver;
        }

        public bool Applicable(Type request)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var contract in _contracts)
            {
                if (contract.IsAssignableFrom(request)) return true;
            }

            return false;
        }

        public abstract object GetInstance(Type contract, IDependencyScope scope);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object Resolve(Type contract, IDependencyScope scope) => _resolver.Resolve(contract, scope);
        
        public abstract void Dispose();
    }
}