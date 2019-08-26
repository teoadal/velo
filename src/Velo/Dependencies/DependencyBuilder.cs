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
        private readonly Dictionary<Type, IDependency> _concreteDependency;
        private readonly List<IDependency> _dependencies;

        public DependencyBuilder(int capacity = 50)
        {
            _concreteDependency = new Dictionary<Type, IDependency>(capacity);
            _dependencies = new List<IDependency>(capacity);
        }

        #region AddDependency

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyBuilder AddDependency(IDependency dependency)
        {
            _dependencies.Add(dependency);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyBuilder AddDependency(Type contract, IDependency dependency)
        {
            if (!_concreteDependency.ContainsKey(contract))
            {
                _concreteDependency.Add(contract, dependency);
            }

            _dependencies.Add(dependency);

            return this;
        }

        #endregion

        #region AddFactory

        public DependencyBuilder AddFactory<TContract>()
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorFactory(new[] {contract}, contract);

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddFactory<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorFactory(new[] {contract}, typeof(TImplementation));

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddFactory<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderFactory<TContract>(new[] {contract}, builder);

            return AddDependency(contract, dependency);
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
            return AddDependency(new GenericSingleton(genericType, true));
        }

        #endregion

        public DependencyBuilder AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new InstanceSingleton(new[] {contract}, instance);

            return AddDependency(contract, dependency);
        }

        #region AddSingleton

        public DependencyBuilder AddSingleton<TContract>()
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, contract);

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddSingleton<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, typeof(TImplementation));

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder);

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddSingleton(Type contract, Type implementation)
        {
            var dependency = new SimpleDependency(contract, implementation);
            return AddDependency(contract, dependency);
        }

        #endregion

        #region AddScope

        public DependencyBuilder AddScope<TContract>()
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, contract, true);

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddScope<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new SimpleDependency(contract, typeof(TImplementation), true);

            return AddDependency(contract, dependency);
        }

        public DependencyBuilder AddScope<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(new[] {contract}, builder, true);

            return AddDependency(contract, dependency);
        }

        #endregion

        public DependencyContainer BuildContainer()
        {
            return new DependencyContainer(_dependencies, _concreteDependency);
        }

        public DependencyBuilder Configure(Action<DependencyConfigurator> configure)
        {
            var configurator = new DependencyConfigurator();

            configure(configurator);

            var dependency = configurator.Build();
            return AddDependency(dependency);
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