using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Dependencies.Factories;
using Velo.Dependencies.Resolvers;
using Velo.Dependencies.Scan;
using Velo.Dependencies.Singletons;
using Velo.Dependencies.Transients;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyBuilder
    {
        private readonly List<IDependency> _dependencies;
        private readonly Dictionary<string, IDependency> _dependenciesWithName;

        public DependencyBuilder(int capacity = 50)
        {
            _dependencies = new List<IDependency>(capacity);
            _dependenciesWithName = new Dictionary<string, IDependency>(capacity / 10);
        }

        public DependencyBuilder AddDependency(IDependency dependency, string name = null)
        {
            return Register(dependency, name);
        }

        #region AddGeneric

        public DependencyBuilder AddGenericScope(Type genericContract, Type genericImplementation = null)
        {
            CheckIsGenericTypeDefinition(genericContract);
            CheckIsGenericTypeDefinition(genericImplementation);

            var contracts = new[] {genericContract};

            var dependency = new GenericTransient(contracts, genericImplementation);
            return Register(dependency, scopeDependency: true);
        }

        public DependencyBuilder AddGenericSingleton(Type genericContract, Type genericImplementation = null)
        {
            CheckIsGenericTypeDefinition(genericContract);
            CheckIsGenericTypeDefinition(genericImplementation);

            var contracts = new[] {genericContract};

            var dependency = new GenericSingleton(contracts, genericImplementation);
            return Register(dependency);
        }

        public DependencyBuilder AddGenericTransient(Type genericContract, Type genericImplementation = null)
        {
            CheckIsGenericTypeDefinition(genericContract);
            CheckIsGenericTypeDefinition(genericImplementation);

            var contracts = new[] {genericContract};

            var dependency = new GenericTransient(contracts, genericImplementation);
            return Register(dependency);
        }

        #endregion

        public DependencyBuilder AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new InstanceSingleton(new[] {contract}, instance);
            return Register(dependency);
        }

        #region AddScope

        public DependencyBuilder AddScope<TContract>(string name = null, bool compiled = true)
        {
            var contract = Typeof<TContract>.Raw;
            var contracts = new[] {contract};
            
            var dependency = compiled
                ? (IDependency) new CompiledTransient(contracts, contract)
                : new ActivatorTransient(contracts, contract);

            return Register(dependency, name, true);
        }

        public DependencyBuilder AddScope<TContract, TImplementation>(string name = null, bool compiled = true)
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var implementation = typeof(TImplementation);

            var dependency = compiled
                ? (IDependency) new CompiledTransient(contracts, implementation)
                : new ActivatorTransient(contracts, implementation);

            return Register(dependency, name, true);
        }

        public DependencyBuilder AddScope<TContract>(Func<DependencyContainer, TContract> builder, string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new BuilderTransient<TContract>(new[] {contract}, builder);
            return Register(dependency, name, true);
        }

        #endregion

        #region AddSingleton

        public DependencyBuilder AddSingleton<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new ActivatorSingleton(new[] {contract}, contract);
            return Register(dependency, name);
        }

        public DependencyBuilder AddSingleton<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var implementation = typeof(TImplementation);

            var dependency = new ActivatorSingleton(contracts, implementation);
            return Register(dependency, name);
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder,
            string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);
            return Register(dependency, name);
        }

        public DependencyBuilder AddSingleton(Type contract, Type implementation, string name = null)
        {
            var dependency = new ActivatorSingleton(new[] {contract}, implementation);
            return Register(dependency, name);
        }

        #endregion

        #region AddTransient

        public DependencyBuilder AddTransient<TContract>(string name = null, bool compiled = true)
        {
            var contract = Typeof<TContract>.Raw;
            var contracts = new[] {contract};

            var dependency = compiled
                ? (IDependency) new CompiledTransient(contracts, contract)
                : new ActivatorTransient(contracts, contract);

            return Register(dependency, name);
        }

        public DependencyBuilder AddTransient<TContract, TImplementation>(string name = null, bool compiled = true)
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var implementation = typeof(TImplementation);

            var dependency = compiled
                ? (IDependency) new CompiledTransient(contracts, implementation)
                : new ActivatorTransient(contracts, implementation);

            return Register(dependency, name);
        }

        public DependencyBuilder AddTransient<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderTransient<TContract>(new[] {contract}, builder);

            return Register(dependency);
        }

        #endregion

        public DependencyContainer BuildContainer()
        {
            Register(new ArrayFactory(_dependencies));

            var container = new DependencyContainer(_dependencies, _dependenciesWithName);

            foreach (var resolver in _dependencies)
            {
                resolver.Init(container);
            }

            return container;
        }

        public DependencyBuilder Configure(Action<DependencyConfigurator> configure)
        {
            var configurator = new DependencyConfigurator();

            configure(configurator);

            var (dependency, scope, name) = configurator.Build();
            Register(dependency, scope, name);

            return this;
        }

        public DependencyBuilder Scan(Action<AssemblyScanner> configure)
        {
            var configurator = new AssemblyScanner(this);

            configure(configurator);
            configurator.Scan();

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DependencyBuilder Register(IDependency dependency, string name = null, bool scopeDependency = false)
        {
            var resolver = scopeDependency
                ? (IDependency) new ScopeResolver(dependency)
                : new DefaultResolver(dependency);

            if (!string.IsNullOrWhiteSpace(name))
            {
                _dependenciesWithName.Add(name, resolver);
            }

            _dependencies.Add(resolver);

            return this;
        }

        private static void CheckIsGenericTypeDefinition(Type type)
        {
            if (type != null && !type.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"{type.Name} is not generic type definition");
            }
        }
    }
}