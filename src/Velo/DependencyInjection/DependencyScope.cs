using System;
using System.Collections.Generic;
using Velo.Collections;

namespace Velo.DependencyInjection
{
    public interface IDependencyScope : IServiceProvider, IDisposable
    {
        IDependencyScope StartScope();
    }

    internal sealed class DependencyScope : IDependencyScope
    {
        private IDependencyEngine _engine;
        private object _lock;

        private Dictionary<Type, object> _dependencies;

        public DependencyScope(IDependencyEngine engine, object @lock)
        {
            _engine = engine;
            _lock = @lock;
            _dependencies = new Dictionary<Type, object>();
        }

        public IDependencyScope StartScope()
        {
            return new DependencyScope(_engine, _engine);
        }

        public object? GetService(Type contract)
        {
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
            _engine = null!;
            _lock = null!;

            CollectionUtils.DisposeValuesIfDisposable(_dependencies);

            _dependencies.Clear();
            _dependencies = null!;
        }
    }
}