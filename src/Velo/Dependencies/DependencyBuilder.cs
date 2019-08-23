using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Velo.Dependencies.Factories;
using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    public sealed class DependencyBuilder
    {
        private readonly List<DependencyConfigurator> _configurators;
        private readonly List<IDependency> _dependencies;

        public DependencyBuilder()
        {
            _configurators = new List<DependencyConfigurator>();
            _dependencies = new List<IDependency>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyBuilder AddDependency(IDependency dependency)
        {
            _dependencies.Add(dependency);
            return this;
        }

        public DependencyBuilder AddFactory<TContract>() where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new ActivatorFactory(contracts, contracts[0]);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddFactory<TContract, TImplementation>()
            where TContract : class
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new ActivatorFactory(contracts, typeof(TImplementation));

            return AddDependency(dependency);
        }

        public DependencyBuilder AddFactory<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new BuilderFactory<TContract>(contracts, builder);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddGenericFactory(Type genericType)
        {
            return AddDependency(new GenericFactory(genericType));
        }

        public DependencyBuilder AddGenericSingleton(Type genericType)
        {
            return AddDependency(new GenericSingleton(genericType));
        }
        
        public DependencyBuilder AddSingleton<TContract>() where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new ActivatorSingleton(contracts, contracts[0]);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddSingleton<TContract, TImplementation>()
            where TContract : class
            where TImplementation : TContract
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new ActivatorSingleton(contracts, typeof(TImplementation));

            return AddDependency(dependency);
        }

        public DependencyBuilder AddSingleton<TContract>(TContract instance) where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new InstanceSingleton(contracts, instance);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var dependency = new BuilderSingleton<TContract>(contracts, builder);

            return AddDependency(dependency);
        }

        public DependencyContainer Build()
        {
            foreach (var configurator in _configurators)
            {
                var dependency = configurator.Build();
                _dependencies.Add(dependency);
            }

            var container = new DependencyContainer(_dependencies);

            _configurators.Clear();
            _dependencies.Clear();

            return container;
        }

        public DependencyConfigurator Configure()
        {
            var configurator = new DependencyConfigurator();

            _configurators.Add(configurator);

            return configurator;
        }
    }
}