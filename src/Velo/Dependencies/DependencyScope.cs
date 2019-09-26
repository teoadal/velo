using System;
using System.Collections.Generic;
using Velo.Dependencies.Resolvers;

namespace Velo.Dependencies
{
    public sealed class DependencyScope : IDisposable
    {
        public static DependencyScope Current => _current;

        public readonly string Name;

        [ThreadStatic] 
        private static DependencyScope _current;

        private List<IDependencyResolver> _scopeResolvers;
        private DependencyScope _parent;

        private bool _disposed;
        
        internal DependencyScope(string name)
        {
            Name = name;

            _parent = _current;
            _current = this;
            _scopeResolvers = new List<IDependencyResolver>();
        }

        internal static void Register(IDependencyResolver resolver)
        {
            if (_current == null)
            {
                throw new InvalidOperationException("Scope is not started");
            }
            
            if (_current._disposed)
            {
                throw new ObjectDisposedException(nameof(DependencyScope));
            }

            var scopeDependencies = _current._scopeResolvers;

            if (scopeDependencies.Contains(resolver))
            {
                throw new InvalidOperationException($"Dependency {resolver} already exists in scope {_current}");
            }
            
            scopeDependencies.Add(resolver);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _current = _parent;
            _disposed = true;
            _parent = null;

            foreach (var dependency in _scopeResolvers)
            {
                dependency.Destroy();
            }

            _scopeResolvers.Clear();
            _scopeResolvers = null;
        }

        public override string ToString()
        {
            return _parent == null
                ? Name
                : $"{_parent} -> {Name}";
        }
    }
}