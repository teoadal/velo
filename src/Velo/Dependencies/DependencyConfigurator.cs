using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Dependencies.Singletons;
using Velo.Dependencies.Transients;
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
        private bool _scope;
        private bool _singleton;
        private bool _transient;

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
            if (_implementation != null && !_implementation.IsAssignableFrom(contract) && !_implementation.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"{contract} is not assignable from {_implementation}");
            }

            _contracts.Add(contract);
            return this;
        }

        public DependencyConfigurator Builder<T>(Func<DependencyContainer, T> builder)
        {
            CheckResolveRule();
            CheckContracts(typeof(T));

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
            CheckResolveRule();
            CheckContracts(implementation);

            _implementation = implementation;
            return this;
        }

        public DependencyConfigurator Instance(object instance)
        {
            CheckLifetimeRule();
            CheckResolveRule();

            _instance = instance;
            _singleton = true;
            return this;
        }

        public DependencyConfigurator Name(string name)
        {
            if (!string.IsNullOrEmpty(_name))
            {
                throw Error.InconsistentOperation("name already exists");
            }

            _name = name;
            return this;
        }

        public DependencyConfigurator Scope(bool value = true)
        {
            CheckLifetimeRule();

            _singleton = value;
            _scope = value;

            return this;
        }

        public DependencyConfigurator Singleton(bool value = true)
        {
            CheckLifetimeRule();

            _singleton = value;
            return this;
        }

        public DependencyConfigurator Transient(bool value = true)
        {
            CheckLifetimeRule();

            _transient = value;
            return this;
        }

        internal (IDependency, string, bool) Build()
        {
            var contracts = CollectContracts();

            IDependency dependency;
            if (_instance != null) dependency = new InstanceSingleton(contracts, _instance);
            else if (_singleton) dependency = BuildSingletonDependency(contracts);
            else if (_transient) dependency = BuildTransientDependency(contracts);
            else throw Error.InconsistentOperation("Lifetime not configured");

            return (dependency, _name, _scope);
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

            throw Error.InconsistentOperation("invalid singleton configuration");
        }

        private IDependency BuildTransientDependency(Type[] contracts)
        {
            if (_builder != null)
            {
                var builderResult = _builder.GetType().GetGenericArguments()[1];
                var builderType = typeof(BuilderTransient<>).MakeGenericType(builderResult);

                return (IDependency) Activator.CreateInstance(builderType, contracts, _builder);
            }

            if (_implementation != null)
            {
                return new ActivatorTransient(contracts, _implementation);
            }

            throw Error.InconsistentOperation("invalid transient configuration");
        }

        private void CheckContracts(Type implementation)
        {
            if (_contracts.Count == 0) return;

            foreach (var contract in _contracts)
            {
                if (contract.IsGenericTypeDefinition) continue;
                if (contract.IsAssignableFrom(implementation)) continue;
                throw Error.InvalidOperation($"{contract} is not assignable from {implementation}");
            }
        }

        private void CheckLifetimeRule()
        {
            if (_scope || _singleton || _transient)
            {
                throw Error.InconsistentOperation("lifetime already configured");
            }
        }

        private void CheckResolveRule()
        {
            if (_implementation != null || _builder != null || _instance != null)
            {
                throw Error.InconsistentOperation("implementation, builder or instance already set");
            }
        }

        private Type[] CollectContracts()
        {
            if (_contracts.Count > 0) return _contracts.ToArray();

            if (_instance != null) return new[] {_instance.GetType()};
            if (_implementation != null) return new[] {_implementation};

            throw Error.InconsistentOperation("set contracts");
        }
    }
}