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
        private readonly List<IDependency> _dependencies;

        public DependencyBuilder()
        {
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

        public DependencyContainer BuildContainer()
        {
            return new DependencyContainer(_dependencies);
        }

        public DependencyBuilder Configure(Action<DependencyConfigurator> configure)
        {
            var configurator = new DependencyConfigurator();

            configure(configurator);

            var dependency = configurator.Build();
            return AddDependency(dependency);
        }
    }
}