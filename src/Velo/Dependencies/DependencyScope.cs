using System;
using System.Collections.Generic;

namespace Velo.Dependencies
{
    public sealed class DependencyScope : IDisposable
    {
        public static DependencyScope Current => _current;

        public string Name { get; private set; }

        [ThreadStatic] private static DependencyScope _current;

        private bool _disposed;
        private Dictionary<Type, IDisposable> _instances;
        private DependencyScope _parent;

        internal DependencyScope(string name)
        {
            Name = name;
            _instances = new Dictionary<Type, IDisposable>();

            if (_current == null) _parent = _current;
            _current = this;
        }

        public void Add(Type contract, IDisposable instance)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DependencyScope));
            }

            if (_instances.ContainsKey(contract))
            {
                throw new InvalidOperationException($"Current scope already contains {contract}");
            }

            if (_parent != null && _parent.Contains(contract))
            {
                throw new InvalidOperationException($"Parent scope already contains {contract}");
            }

            _instances.Add(contract, instance);
        }

        public bool Contains(Type contract)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DependencyScope));
            }

            if (_instances.ContainsKey(contract)) return true;
            return _parent != null && _parent.Contains(contract);
        }

        public bool TryGet(Type contract, out IDisposable instance)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DependencyScope));
            }

            if (_instances.TryGetValue(contract, out instance)) return true;
            return _parent != null && _parent.TryGet(contract, out instance);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _current = _parent;
            _disposed = true;
            _parent = null;

            foreach (var instance in _instances.Values)
            {
                instance.Dispose();
            }

            _instances.Clear();
            _instances = null;
        }

        public override string ToString()
        {
            return _parent == null
                ? Name
                : $"{_parent} -> {Name}";
        }
    }
}