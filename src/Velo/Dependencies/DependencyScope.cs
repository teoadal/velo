using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyScope : IDisposable
    {
        public static DependencyScope Current => _current;

        public readonly string Name;

        [ThreadStatic] private static DependencyScope _current;

        private readonly DependencyContainer _container;
        private readonly Dictionary<IDependency, object> _dependencies;
        private bool _disposed;
        private DependencyScope _parent;
        private readonly HashSet<Type> _resolveInProgress;

        internal DependencyScope(DependencyContainer container, string name)
        {
            Name = name;

            _container = container;
            _parent = _current;
            _current = this;

            _dependencies = new Dictionary<IDependency, object>();
            _resolveInProgress = new HashSet<Type>();
        }

        internal object GetOrAdd(IDependency dependency, Type contract)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyScope));

            if (TryGetInstance(dependency, out var exists))
            {
                return exists;
            }

            BeginResolving(contract, dependency);

            var instance = dependency.Resolve(contract, _container);
            _dependencies.Add(dependency, instance);

            ResolvingComplete(contract);

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BeginResolving(Type contract, IDependency dependency)
        {
            if (_resolveInProgress.Add(contract)) return;
            throw Error.CircularDependency(dependency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResolvingComplete(Type contract)
        {
            _resolveInProgress.Remove(contract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetInstance(IDependency dependency, out object instance)
        {
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