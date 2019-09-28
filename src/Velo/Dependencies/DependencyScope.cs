using System;
using System.Collections.Generic;
using System.Resources;
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
        private List<IDependencyResolver> _scopeResolvers;
        private bool _disposed;

        internal DependencyScope(string name)
        {
            Name = name;

            _parent = _current;
            _current = this;
            _scopeResolvers = new List<IDependencyResolver>();
        }

        internal bool Contains(IDependencyResolver resolver)
        {
            if (_scopeResolvers.Contains(resolver)) return true;
            return _parent?.Contains(resolver) ?? false;
        }
        
        internal bool TryAdd(IDependencyResolver resolver)
        {
            if (_current._disposed) throw Error.Disposed(nameof(DependencyScope));

            if (Contains(resolver)) return false;
            _scopeResolvers.Add(resolver);
            return true;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _current = _parent;
            _disposed = true;
            _parent = null;

            foreach (var resolver in _scopeResolvers)
            {
                resolver.Destroy();
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