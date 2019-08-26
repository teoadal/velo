using System;
using System.Collections.Generic;

namespace Velo.Dependencies
{
    public sealed class DependencyScope : IDisposable
    {
        public static DependencyScope Current => _current;

        public string Name { get; private set; }

        [ThreadStatic] 
        private static DependencyScope _current;

        private List<IDependency> _scopeDependencies;
        private bool _disposed;
        private DependencyScope _parent;

        internal DependencyScope(string name)
        {
            Name = name;

            _parent = _current;
            _current = this;
            _scopeDependencies = new List<IDependency>();
        }

        public static void Add(IDependency dependency)
        {
            if (_current == null)
            {
                throw new InvalidOperationException($"Scope is not started");
            }

            var currentScope = _current;

            if (currentScope._disposed)
            {
                throw new ObjectDisposedException(nameof(DependencyScope));
            }

            var scopeDependencies = currentScope._scopeDependencies;

            if (scopeDependencies.Contains(dependency))
            {
                throw new InvalidOperationException($"Dependency {dependency} already exists in scope {currentScope}");
            }

            scopeDependencies.Add(dependency);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _current = _parent;
            _disposed = true;
            _parent = null;

            foreach (var dependency in _scopeDependencies)
            {
                dependency.Destroy();
            }

            _scopeDependencies.Clear();
            _scopeDependencies = null;
        }

        public override string ToString()
        {
            return _parent == null
                ? Name
                : $"{_parent} -> {Name}";
        }
    }
}