using System;
using System.Collections.Generic;
using Velo.Collections;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public interface IDependencyEngine
    {
        LocalVector<IDependency> GetApplicable(Type contract);

        IDependency GetDependency(Type contract, bool required = false);
    }
    
    internal sealed class DependencyEngine : IDependencyEngine, IDisposable
    {
        private readonly List<IDependency> _dependencies;
        private readonly List<IDependencyFactory> _factories;
        
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
        
        public LocalVector<IDependency> GetApplicable(Type contract)
        {
            var vector = new LocalVector<IDependency>();

            foreach (var dependency in _dependencies)
            {
                if (dependency.Applicable(contract))
                {
                    vector.Add(dependency);
                }
            }

            foreach (var factory in _factories)
            {
                if (!factory.Applicable(contract)) continue;

                var dependency = factory.BuildDependency(contract, this);
                _dependencies.Add(dependency);
                vector.Add(dependency);
            }

            return vector;
        }

        public IDependency GetDependency(Type contract, bool required = false)
        {
            if (_resolvedDependencies.TryGetValue(contract, out var existsDependency))
            {
                return existsDependency;
            }

            foreach (var dependency in _dependencies)
            {
                if (!dependency.Applicable(contract)) continue;

                _resolvedDependencies.Add(contract, dependency);
                return dependency;
            }

            foreach (var factory in _factories)
            {
                if (!factory.Applicable(contract)) continue;

                var dependency = factory.BuildDependency(contract, this);
                _dependencies.Add(dependency);
                _resolvedDependencies.Add(contract, dependency);
                return dependency;
            }

            if (required)
            {
                throw Error.NotFound($"Dependency with contract {ReflectionUtils.GetName(contract)} is not registered");
            }

            return null;
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