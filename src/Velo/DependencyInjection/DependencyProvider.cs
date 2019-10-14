using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Engine;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyProvider : IServiceProvider, IDisposable
    {
        public event Action<DependencyProvider> Destroy;

        public readonly object SyncRoot;
        
        private RuntimeEngine _engine;
        private DependencyProvider _parent;
        private bool _disposed;

        internal DependencyProvider(RuntimeEngine engine)
        {
            _engine = engine;
            SyncRoot = new object();
        }

        private DependencyProvider(DependencyProvider parent)
        {
            _parent = parent;
            SyncRoot = parent.SyncRoot;
        }

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
            lock (SyncRoot)
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
            lock (SyncRoot)
            {
                var dependency = GetDependency(contract, false);
                return dependency?.GetInstance(this);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Dependency GetDependency(Type contract, bool throwIfNotRegistered = true)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyProvider));

            var engine = _engine ?? _parent._engine;
            return engine.GetDependency(contract, throwIfNotRegistered);
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