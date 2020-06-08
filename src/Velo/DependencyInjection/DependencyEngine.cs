using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Collections;
using Velo.Collections.Local;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public interface IDependencyEngine : IDisposable
    {
        bool Contains(Type type);

        IDependency[] GetApplicable(Type contract);

        IDependency? GetDependency(Type contract);

        IDependency GetRequiredDependency(Type contract);
    }

    internal sealed class DependencyEngine : IDependencyEngine
    {
        private readonly List<IDependency> _dependencies;
        private readonly List<IDependencyFactory> _factories;

        private bool _disposed;
        private readonly Dictionary<Type, IDependency> _resolvedDependencies;

        public DependencyEngine(int capacity)
        {
            _dependencies = new List<IDependency>(capacity);

            _factories = new List<IDependencyFactory>(4)
            {
                new ArrayFactory()
            };

            _resolvedDependencies = new Dictionary<Type, IDependency>();
        }

        public void AddDependency(IDependency dependency)
        {
            _dependencies.Add(dependency);
        }

        public void AddFactory(IDependencyFactory factory)
        {
            _factories.Add(factory);
        }

        public bool Contains(Type type)
        {
            EnsureNotDisposed();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var dependency in _dependencies)
            {
                if (dependency.Applicable(type)) return true;
            }

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var factory in _factories)
            {
                if (factory.Applicable(type)) return true;
            }

            return false;
        }

        public IDependency[] GetApplicable(Type contract)
        {
            EnsureNotDisposed();

            var localList = new LocalList<IDependency>();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var dependency in _dependencies)
            {
                if (dependency.Applicable(contract))
                {
                    localList.Add(dependency);
                }
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var factory in _factories)
            {
                if (!factory.Applicable(contract)) continue;

                var dependency = factory.BuildDependency(contract, this);
                _dependencies.Add(dependency);
                localList.Add(dependency);
            }

            return localList.ToArray();
        }

        public IDependency? GetDependency(Type contract)
        {
            EnsureNotDisposed();

            if (_resolvedDependencies.TryGetValue(contract, out var existsDependency))
            {
                return existsDependency;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var dependency in _dependencies)
            {
                if (!dependency.Applicable(contract)) continue;

                _resolvedDependencies.Add(contract, dependency);
                return dependency;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var factory in _factories)
            {
                if (!factory.Applicable(contract)) continue;

                var dependency = factory.BuildDependency(contract, this);
                _dependencies.Add(dependency);
                _resolvedDependencies.Add(contract, dependency);
                return dependency;
            }

            return null;
        }

        public IDependency GetRequiredDependency(Type contract)
        {
            var dependency = GetDependency(contract);
            return dependency ?? throw Error.DependencyNotRegistered(contract);
        }

        public bool Remove(Type contract)
        {
            _resolvedDependencies.Remove(contract);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var dependency in _dependencies)
            {
                if (!dependency.Applicable(contract)) continue;

                _dependencies.Remove(dependency);
                return true;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var factory in _factories)
            {
                if (!factory.Applicable(contract)) continue;

                _factories.Remove(factory);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(IDependencyEngine));
        }

        public void Dispose()
        {
            if (_disposed) return;

            foreach (var dependency in _dependencies)
            {
                dependency.Dispose();
            }

            _dependencies.Clear();

            CollectionUtils.DisposeValuesIfDisposable(_factories);
            _factories.Clear();

            _resolvedDependencies.Clear();

            _disposed = true;
        }
    }
}