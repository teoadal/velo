using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Dependencies
{
    public interface IDependency : IDisposable
    {
        Type[] Contracts { get; }

        Type Implementation { get; }

        DependencyLifetime Lifetime { get; }

        bool Applicable(Type contract);

        void Init(IDependencyEngine engine);
        
        object GetInstance(Type contract, IServiceProvider services);
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

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public Type[] Contracts => _contracts;

        public Type Implementation => _resolver.Implementation;

        public DependencyLifetime Lifetime { get; }

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private Type[] _contracts;

        private DependencyResolver _resolver;
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        protected Dependency(Type[] contracts, DependencyResolver resolver, DependencyLifetime lifetime)
        {
            Lifetime = lifetime;
            _contracts = contracts;
            _resolver = resolver;
        }

        public bool Applicable(Type request)
        {
            // ReSharper disable once InvertIf
            if (request.IsInterface)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var contract in _contracts)
                {
                    if (request.IsAssignableFrom(contract)) return true;
                }
            }

            return Array.IndexOf(_contracts, request) != -1;
        }

        public void Init(IDependencyEngine engine)
        {
            _resolver.Init(Lifetime, engine);
        }

        public abstract object GetInstance(Type contract, IServiceProvider services);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected object Resolve(Type contract, IServiceProvider services) => _resolver.Resolve(contract, services);

        public abstract void Dispose();
    }
}