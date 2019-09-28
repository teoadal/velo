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
        private readonly List<IDependencyResolver> _resolvers;

        public DependencyBuilder(int capacity = 50)
        {
            _resolvers = new List<IDependencyResolver>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyBuilder AddDependency(IDependency dependency, string name = null, bool scopeDependency = false)
        {
            var resolver = scopeDependency
                ? (IDependencyResolver) new ScopeDependencyResolver(dependency, name)
                : new DependencyResolver(dependency, name);

            _resolvers.Add(resolver);

            return this;
        }

        #region AddGeneric

        public DependencyBuilder AddGenericScope(Type genericContract, Type genericImplementation = null)
        {
            var contracts = new[] {genericContract};

            var dependency = new GenericSingleton(contracts, genericImplementation);
            return AddDependency(dependency, scopeDependency: true);
        }

        public DependencyBuilder AddGenericSingleton(Type genericContract, Type genericImplementation = null)
        {
            var contracts = new[] {genericContract};

            var dependency = new GenericSingleton(contracts, genericImplementation);
            return AddDependency(dependency);
        }

        public DependencyBuilder AddGenericTransient(Type genericContract, Type genericImplementation = null)
        {
            var contracts = new[] {genericContract};

            var dependency = new GenericTransient(contracts, genericImplementation);
            return AddDependency(dependency);
        }

        #endregion

        public DependencyBuilder AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new InstanceSingleton(new[] {contract}, instance);
            return AddDependency(dependency);
        }

        #region AddScope

        public DependencyBuilder AddScope<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new CompiledSingleton(new[] {contract}, contract);
            return AddDependency(dependency, name, true);
        }

        public DependencyBuilder AddScope<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            var dependency = new CompiledSingleton(new[] {contract}, implementation);
            return AddDependency(dependency, name, true);
        }

        public DependencyBuilder AddScope<TContract>(Func<DependencyContainer, TContract> builder, string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);
            return AddDependency(dependency, name, true);
        }

        #endregion

        #region AddSingleton

        public DependencyBuilder AddSingleton<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new ActivatorSingleton(new[] {contract}, contract);
            return AddDependency(dependency, name);
        }

        public DependencyBuilder AddSingleton<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var implementation = typeof(TImplementation);

            var dependency = new ActivatorSingleton(contracts, implementation);
            return AddDependency(dependency, name);
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder,
            string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);
            return AddDependency(dependency, name);
        }

        public DependencyBuilder AddSingleton(Type contract, Type implementation, string name = null)
        {
            var dependency = new ActivatorSingleton(new[] {contract}, implementation);
            return AddDependency(dependency, name);
        }

        #endregion

        #region AddTransient

        public DependencyBuilder AddTransient<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;

            var dependency = new CompiledTransient(new[] {contract}, contract);
            return AddDependency(dependency, name);
        }

        public DependencyBuilder AddTransient<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var implementation = typeof(TImplementation);

            var dependency = new CompiledTransient(contracts, implementation);
            return AddDependency(dependency, name);
        }

        public DependencyBuilder AddTransient<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderTransient<TContract>(new[] {contract}, builder);

            return AddDependency(dependency);
        }

        #endregion

        public DependencyContainer BuildContainer()
        {
            AddDependency(new ArrayFactory(_resolvers));
            
            var container = new DependencyContainer(_resolvers);

            foreach (var resolver in _resolvers)
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
            AddDependency(dependency, scope, name);

            return this;
        }

        public DependencyBuilder Scan(Action<AssemblyScanner> configure)
        {
            var configurator = new AssemblyScanner(this);

            configure(configurator);
            configurator.Scan();

            return this;
        }
    }
}