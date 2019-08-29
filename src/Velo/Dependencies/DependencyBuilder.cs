using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Velo.Dependencies.Factories;
using Velo.Dependencies.Scan;
using Velo.Dependencies.Singletons;
using Velo.Dependencies.Transients;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyBuilder
    {
        private readonly List<DependencyResolver> _resolvers;

        public DependencyBuilder(int capacity = 50)
        {
            _resolvers = new List<DependencyResolver>(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyBuilder AddDependency(IDependency dependency)
        {
            _resolvers.Add(new DependencyResolver(dependency));
            return this;
        }

        #region AddGeneric

        public DependencyBuilder AddGenericScope(Type genericType)
        {
            return AddDependency(new GenericSingleton(genericType));
        }

        public DependencyBuilder AddGenericSingleton(Type genericType)
        {
            return AddDependency(new GenericSingleton(genericType));
        }

        public DependencyBuilder AddGenericTransient(Type genericType)
        {
            return AddDependency(new GenericTransient(genericType));
        }

        #endregion

        #region AddScope

        public DependencyBuilder AddScope<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, contract);

            RegisterResolver(dependency, name, true);
            return this;
        }

        public DependencyBuilder AddScope<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, typeof(TImplementation));

            RegisterResolver(dependency, name, true);
            return this;
        }

        public DependencyBuilder AddScope<TContract>(Func<DependencyContainer, TContract> builder, string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);

            RegisterResolver(dependency, name, true);
            return this;
        }

        #endregion

        public DependencyBuilder AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new InstanceSingleton(new[] {contract}, instance);

            RegisterResolver(dependency);
            return this;
        }

        #region AddSingleton

        public DependencyBuilder AddSingleton<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, contract);

            RegisterResolver(dependency, name);
            return this;
        }

        public DependencyBuilder AddSingleton<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, typeof(TImplementation));

            RegisterResolver(dependency, name);
            return this;
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder,
            string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);

            RegisterResolver(dependency, name);
            return this;
        }

        public DependencyBuilder AddSingleton(Type contract, Type implementation, string name = null)
        {
            var dependency = new SimpleDependency(contract, implementation);

            RegisterResolver(dependency, name);
            return this;
        }

        #endregion

        #region AddTransient

        public DependencyBuilder AddTransient<TContract>()
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorTransient(new[] {contract}, contract);

            RegisterResolver(dependency);
            return this;
        }

        public DependencyBuilder AddTransient<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorTransient(new[] {contract}, typeof(TImplementation));

            RegisterResolver(dependency);
            return this;
        }

        public DependencyBuilder AddTransient<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderTransient<TContract>(new[] {contract}, builder);

            RegisterResolver(dependency);
            return this;
        }

        #endregion

        public DependencyContainer BuildContainer()
        {
            AddDependency(new ArrayFactory(_resolvers));
            return new DependencyContainer(_resolvers);
        }

        public DependencyBuilder Configure(Action<DependencyConfigurator> configure)
        {
            var configurator = new DependencyConfigurator();

            configure(configurator);

            var resolver = configurator.Build();
            _resolvers.Add(resolver);

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
        private void RegisterResolver(IDependency dependency, string name = null, bool scopeDependency = false)
        {
            var resolver = new DependencyResolver(dependency, name, scopeDependency);
            _resolvers.Add(resolver);
        }
    }
}