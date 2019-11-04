using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Engines;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyProvider : IServiceProvider, IDisposable
    {
        public event Action<DependencyProvider> Destroy;

        private bool _disposed;
        private RuntimeEngine _engine;
        private DependencyProvider _parent;
        private readonly object _lock;

        internal DependencyProvider(Dictionary<Type, DependencyDescription> descriptions,
            List<ResolverFactory> factories)
        {
            AddSelfDependency(descriptions);

            using (var engineBuilder = new DependencyEngineBuilder(descriptions, factories))
            {
                _engine = engineBuilder.Build();
            }
            
            _lock = new object();
        }

        private DependencyProvider(DependencyProvider parent)
        {
            _parent = parent;
            _lock = parent._lock;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyProvider CreateScope()
        {
            return new DependencyProvider(this);
        }

        public T GetRequiredService<T>()
        {
            return (T) GetRequiredService(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetRequiredService(Type contract)
        {
            lock (_lock)
            {
                var dependency = GetDependency(contract);
                return dependency.GetInstance(this);
            }
        }

        public T GetService<T>()
        {
            return (T) GetService(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object GetService(Type contract)
        {
            lock (_lock)
            {
                var dependency = GetDependency(contract, false);
                return dependency?.GetInstance(this);
            }
        }

        private void AddSelfDependency(Dictionary<Type, DependencyDescription> descriptions)
        {
            var providerResolver = new InstanceResolver(this);

            var description = new DependencyDescription(providerResolver);
            descriptions.Add(Typeof<DependencyProvider>.Raw, description);
            descriptions.Add(Typeof<IServiceProvider>.Raw, description);   
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Dependency GetDependency(Type contract, bool throwIfNotRegistered = true)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));

            var engine = _engine ?? _parent.GetEngine();
            return engine.GetDependency(contract, throwIfNotRegistered);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DependencyEngine GetEngine()
        {
            // ReSharper disable InconsistentlySynchronizedField
            return _engine ?? _parent.GetEngine();
            // ReSharper restore InconsistentlySynchronizedField
        }

        public void Dispose()
        {
            if (_disposed) return;

            var evt = Destroy;
            evt?.Invoke(this);

            _engine?.Dispose();
            _engine = null;

            _parent = null;

            _disposed = true;
        }
    }
}