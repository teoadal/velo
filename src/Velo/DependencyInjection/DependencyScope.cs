using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Collections;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public interface IDependencyScope : IServiceProvider, IDisposable
    {
        event Action<IDependencyScope> CreateChild;

        event Action<IDependencyScope> Destroy;

        IDependencyScope StartScope();
    }

    internal sealed class DependencyScope : IDependencyScope
    {
        public event Action<IDependencyScope>? CreateChild;

        public event Action<IDependencyScope>? Destroy;

        private IDependencyEngine _engine;
        private object _providerLock;
        private Dictionary<Type, object> _scopeInstances;

        private bool _disposed;

        public DependencyScope(IDependencyEngine engine, object providerLock)
        {
            CreateChild = null;
            Destroy = null;

            _engine = engine;
            _providerLock = providerLock;
            _scopeInstances = new Dictionary<Type, object>(16);
        }

        public IDependencyScope StartScope()
        {
            EnsureNotDisposed();

            var child = new DependencyScope(_engine, _providerLock);

            var evt = CreateChild;
            evt?.Invoke(child);

            return child;
        }

        public object? GetService(Type contract)
        {
            EnsureNotDisposed();

            if (contract == typeof(IServiceProvider)) return this;

            lock (_providerLock)
            {
                if (_scopeInstances.TryGetValue(contract, out var exists))
                {
                    return exists;
                }

                var dependency = _engine.GetDependency(contract);

                if (dependency == null) return null;

                var instance = dependency.GetInstance(contract, this);

                if (dependency.Lifetime == DependencyLifetime.Scoped)
                {
                    _scopeInstances.Add(contract, instance);
                }

                return instance;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(IDependencyScope));
        }

        public void Dispose()
        {
            if (_disposed) return;

            var evt = Destroy;
            evt?.Invoke(this);

            _engine = null!;
            _providerLock = null!;

            CollectionUtils.DisposeValuesIfDisposable(_scopeInstances);
            _scopeInstances.Clear();
            _scopeInstances = null!;

            _disposed = true;
        }
    }
}