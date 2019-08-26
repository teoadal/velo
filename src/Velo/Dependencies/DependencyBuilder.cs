using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Velo.Dependencies.Factories;
using Velo.Dependencies.Scan;
using Velo.Dependencies.Singletons;
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

        #region AddFactory

        public DependencyBuilder AddFactory<TContract>()
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorFactory(new[] {contract}, contract);

            RegisterResolver(contract, dependency);
            return this;
        }

        public DependencyBuilder AddFactory<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorFactory(new[] {contract}, typeof(TImplementation));

            RegisterResolver(contract, dependency);
            return this;
        }

        public DependencyBuilder AddFactory<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderFactory<TContract>(new[] {contract}, builder);

            RegisterResolver(contract, dependency);
            return this;
        }

        #endregion

        #region AddGeneric

        public DependencyBuilder AddGenericFactory(Type genericType)
        {
            return AddDependency(new GenericFactory(genericType));
        }

        public DependencyBuilder AddGenericSingleton(Type genericType)
        {
            return AddDependency(new GenericSingleton(genericType));
        }

        public DependencyBuilder AddGenericScope(Type genericType)
        {
            return AddDependency(new GenericSingleton(genericType));
        }

        #endregion

        public DependencyBuilder AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new InstanceSingleton(new[] {contract}, instance);

            RegisterResolver(contract, dependency);
            return this;
        }

        #region AddSingleton

        public DependencyBuilder AddSingleton<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, contract);

            RegisterResolver(contract, dependency, name);
            return this;
        }

        public DependencyBuilder AddSingleton<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, typeof(TImplementation));

            RegisterResolver(contract, dependency, name);
            return this;
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder, string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);

            RegisterResolver(contract, dependency, name);
            return this;
        }

        public DependencyBuilder AddSingleton(Type contract, Type implementation, string name = null)
        {
            var dependency = new SimpleDependency(contract, implementation);
            
            RegisterResolver(contract, dependency, name);
            return this;
        }

        #endregion

        #region AddScope

        public DependencyBuilder AddScope<TContract>(string name = null)
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, contract);

            RegisterResolver(contract, dependency, name, true);
            return this;
        }

        public DependencyBuilder AddScope<TContract, TImplementation>(string name = null)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, typeof(TImplementation));

            RegisterResolver(contract, dependency, name, true);
            return this;
        }

        public DependencyBuilder AddScope<TContract>(Func<DependencyContainer, TContract> builder, string name = null)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);

            RegisterResolver(contract, dependency, name, true);
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
        private void RegisterResolver(Type contract, IDependency dependency, string dependencyName = null,
            bool isScopeDependency = false)
        {
            var resolver = new DependencyResolver(dependency, dependencyName, isScopeDependency);
            _resolvers.Add(resolver);
        }
    }
}