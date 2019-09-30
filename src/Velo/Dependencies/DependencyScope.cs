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

        public object GetOrAdd(IDependency dependency, Func<IDependency, object> builder)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyScope));

            if (TryGetInstance(dependency, out var exists)) return exists;

            BeginResolving(dependency);

            var instance = builder(dependency);
            _dependencies.Add(dependency, instance);

            ResolvingComplete(dependency);

            return instance;
        }

        /// <summary>
        /// Use for reduce allocation
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public object GetOrAdd<T1, T2, T3>(IDependency dependency, T1 arg1, T2 arg2, T3 arg3,
            Func<T1, T2, T3, object> builder)
        {
            if (_disposed) throw Error.Disposed(nameof(DependencyScope));

            if (TryGetInstance(dependency, out var exists)) return exists;

            BeginResolving(dependency);

            var instance = builder(arg1, arg2, arg3);
            _dependencies.Add(dependency, instance);

            ResolvingComplete(dependency);

            return instance;
        }

        public bool TryGetInstance(IDependency dependency, out object instance)
        {
            if (_current._disposed) throw Error.Disposed(nameof(DependencyScope));

            if (_dependencies.TryGetValue(dependency, out instance)) return true;
            return _parent?.TryGetInstance(dependency, out instance) ?? false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BeginResolving(IDependency dependency)
        {
            if (_resolveInProgress.Add(dependency)) return;
            throw Error.CircularDependency(dependency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResolvingComplete(IDependency dependency)
        {
            _resolveInProgress.Remove(dependency);
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