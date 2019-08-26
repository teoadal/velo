using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Velo.Dependencies.Factories;
using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyConfigurator
    {
        private Delegate _builder;
        private readonly List<Type> _contracts;
        private Type _implementation;
        private object _instance;
        private string _name;
        private bool _isScope;
        private bool _singleton;

        public DependencyConfigurator()
        {
            _contracts = new List<Type>(2);
        }

        public DependencyConfigurator Contract<TContract>()
        {
            return Contract(Typeof<TContract>.Raw);
        }

        public DependencyConfigurator Contracts<TContract1, TContract2>()
        {
            Contract(Typeof<TContract1>.Raw);
            Contract(Typeof<TContract2>.Raw);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyConfigurator Contract(Type contract)
        {
            if (_implementation != null && !_implementation.IsAssignableFrom(contract))
            {
                throw new InvalidOperationException($"{contract} is not assignable from {_implementation}");
            }

            _contracts.Add(contract);
            return this;
        }

        public DependencyConfigurator Builder<T>(Func<DependencyContainer, T> builder)
        {
            if (_implementation != null) throw InconsistentConfiguration();

            if (_contracts.Count > 0)
            {
                var implementation = typeof(T);
                foreach (var contract in _contracts)
                {
                    if (contract.IsAssignableFrom(implementation)) continue;
                    throw new InvalidOperationException($"{contract} is not assignable from {implementation}");
                }
            }

            _builder = builder;
            return this;
        }

        public DependencyConfigurator Implementation<TImplementation>()
        {
            return Implementation(typeof(TImplementation));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyConfigurator Implementation(Type implementation)
        {
            if (_implementation != null || _builder != null || _instance != null)
            {
                throw InconsistentConfiguration();
            }

            if (_contracts.Count > 0)
            {
                foreach (var contract in _contracts)
                {
                    if (contract.IsAssignableFrom(implementation)) continue;
                    throw new InvalidOperationException($"{contract} is not assignable from {implementation}");
                }
            }

            _implementation = implementation;
            return this;
        }

        public DependencyConfigurator Instance(object instance)
        {
            if (_implementation != null || _builder != null || _instance != null)
            {
                throw InconsistentConfiguration();
            }

            _instance = instance;
            return this;
        }

        public DependencyConfigurator Name(string name)
        {
            _name = name;
            return this;
        }
        
        public DependencyConfigurator Scope(bool value = true)
        {
            _singleton = value;
            _isScope = value;

            return this;
        }

        public DependencyConfigurator Singleton(bool value = true)
        {
            _singleton = value;
            return this;
        }

        internal DependencyResolver Build()
        {
            var contracts = _contracts.ToArray();

            IDependency dependency;
            if (_instance != null) dependency = new InstanceSingleton(contracts, _instance);
            else if (_singleton) dependency = BuildSingletonDependency(contracts);
            else dependency = BuildFactoryDependency(contracts);
            
            return new DependencyResolver(dependency, _name, _isScope);
        }

        private static Exception InconsistentConfiguration()
        {
            return new InvalidOperationException("Inconsistent dependency configuration");
        }

        private IDependency BuildSingletonDependency(Type[] contracts)
        {
            if (_builder != null)
            {
                var builderResult = _builder.GetType().GetGenericArguments()[1];
                var builderType = typeof(BuilderSingleton<>).MakeGenericType(builderResult);
                
                return (IDependency) Activator.CreateInstance(builderType, contracts, _builder);
            }

            if (_implementation != null)
            {
                return contracts.Length == 1
                    ? (IDependency) new SimpleDependency(contracts[0], _implementation)
                    : new ActivatorSingleton(contracts, _implementation);
            }

            throw new NotImplementedException();
        }
        
        private IDependency BuildFactoryDependency(Type[] contracts)
        {
            if (_builder != null)
            {
                var builderResult = _builder.GetType().GetGenericArguments()[1];
                var builderType = typeof(BuilderFactory<>).MakeGenericType(builderResult);
                
                return (IDependency) Activator.CreateInstance(builderType, contracts, _builder);
            }

            if (_implementation != null)
            {
                return new ActivatorFactory(contracts, _implementation);
            }

            throw InconsistentConfiguration();
        }
    }
}