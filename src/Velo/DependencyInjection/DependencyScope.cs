using System;
using System.Collections.Generic;
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
        private object _lock;

        private bool _disposed;
        private Dictionary<Type, object> _dependencies;

        public DependencyScope(IDependencyEngine engine, object @lock)
        {
            _engine = engine;
            _lock = @lock;
            _dependencies = new Dictionary<Type, object>();

            CreateChild = null;
            Destroy = null;
        }

        public IDependencyScope StartScope()
        {
            if (_disposed) throw Error.Disposed(nameof(IDependencyScope));

            var child = new DependencyScope(_engine, _lock);

            var evt = CreateChild;
            evt?.Invoke(child);

            return child;
        }

        public object? GetService(Type contract)
        {
            if (_disposed) throw Error.Disposed(nameof(IDependencyScope));

            if (contract == typeof(IDependencyScope)) return this;
            
            lock (_lock)
            {
                if (_dependencies.TryGetValue(contract, out var exists))
                {
                    return exists;
                }

                var dependency = _engine.GetDependency(contract);

                if (dependency == null) return null;

                var instance = dependency.GetInstance(contract, this);

                if (dependency.Lifetime == DependencyLifetime.Scoped)
                {
                    _dependencies.Add(contract, instance);
                }

                return instance;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            var evt = Destroy;
            evt?.Invoke(this);

            _engine = null!;
            _lock = null!;

            CollectionUtils.DisposeValuesIfDisposable(_dependencies);

            _dependencies.Clear();
            _dependencies = null!;

            _disposed = true;
        }
    }
}