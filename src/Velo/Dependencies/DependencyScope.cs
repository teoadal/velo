using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyScope : IDisposable
    {
        public static DependencyScope Current => _current;

        public readonly string Name;

        [ThreadStatic] private static DependencyScope _current;

        private readonly Dictionary<IDependency, object> _dependencies;
        private bool _disposed;
        private DependencyScope _parent;
        private readonly HashSet<IDependency> _resolveInProgress;

        internal DependencyScope(string name)
        {
            Name = name;

            _parent = _current;
            _current = this;

            _dependencies = new Dictionary<IDependency, object>();
            _resolveInProgress = new HashSet<IDependency>();
        }

        internal void Add(IDependency dependency, object instance)
        {
            if (_current._disposed) throw Error.Disposed(nameof(DependencyScope));
            _dependencies.Add(dependency, instance);
        }

        internal void BeginResolving(IDependency dependency)
        {
            if (_resolveInProgress.Add(dependency)) return;
            throw Error.CircularDependency(dependency);
        }

        internal void ResolvingComplete(IDependency dependency)
        {
            _resolveInProgress.Remove(dependency);
        }

        internal bool TryGetInstance(IDependency dependency, out object instance)
        {
            if (_current._disposed) throw Error.Disposed(nameof(DependencyScope));

            if (_dependencies.TryGetValue(dependency, out instance)) return true;
            return _parent?.TryGetInstance(dependency, out instance) ?? false;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _current = _parent;
            _disposed = true;
            _parent = null;

            foreach (var pair in _dependencies)
            {
                var resolver = pair.Key;
                resolver.Destroy();

                var instance = pair.Value;
                if (instance is IDisposable disposable) disposable.Dispose();
            }

            _dependencies.Clear();
        }

        public override string ToString()
        {
            return _parent == null
                ? Name
                : $"{_parent} -> {Name}";
        }
    }
}