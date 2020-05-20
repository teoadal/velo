using System;
using System.Collections.Generic;
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

        private readonly Dictionary<Type, IDependency> _resolvedDependencies;

        public DependencyEngine(int capacity)
        {
            _dependencies = new List<IDependency>(capacity);

            _factories = new List<IDependencyFactory>(4)
            {
                new ArrayFactory(),
                new ReferenceFactory()
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

            if (dependency == null)
            {
                throw Error.NotFound(
                    $"Dependency with contract '{ReflectionUtils.GetName(contract)}' isn't registered");
            }

            return dependency;
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

        public void Dispose()
        {
            foreach (var dependency in _dependencies)
            {
                dependency.Dispose();
            }

            _dependencies.Clear();
            _resolvedDependencies.Clear();
        }
    }
}