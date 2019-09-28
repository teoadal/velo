using System;
using System.Collections.Generic;
using Velo.Dependencies.Resolvers;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyScope : IDisposable
    {
        public static DependencyScope Current => _current;

        public readonly string Name;

        [ThreadStatic] private static DependencyScope _current;
        
        private DependencyScope _parent;
        private Dictionary<IDependencyResolver, object> _scopeInstances;
        private bool _disposed;

        internal DependencyScope(string name)
        {
            Name = name;

            _parent = _current;
            _current = this;
            
            _scopeInstances = new Dictionary<IDependencyResolver, object>();
        }

        internal void Add(IDependencyResolver resolver, object instance)
        {
            if (_current._disposed) throw Error.Disposed(nameof(DependencyScope));
            _scopeInstances.Add(resolver, instance);
        }

        internal bool TryGetInstance(IDependencyResolver resolver, out object instance)
        {
            if (_current._disposed) throw Error.Disposed(nameof(DependencyScope));

            if (_scopeInstances.TryGetValue(resolver, out instance)) return true;
            return _parent?.TryGetInstance(resolver, out instance) ?? false;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _current = _parent;
            _disposed = true;
            _parent = null;

            foreach (var pair in _scopeInstances)
            {
                var resolver = pair.Key;
                resolver.Destroy();

                var instance = pair.Value;
                if (instance is IDisposable disposable) disposable.Dispose();
            }

            _scopeInstances.Clear();
            _scopeInstances = null;
        }

        public override string ToString()
        {
            return _parent == null
                ? Name
                : $"{_parent} -> {Name}";
        }
    }
}