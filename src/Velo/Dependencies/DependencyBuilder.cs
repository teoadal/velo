using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Velo.Dependencies.Factories;
using Velo.Dependencies.Singletons;
using Velo.Utils;

namespace Velo.Dependencies
{
    internal sealed class DependencyBuilder
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
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorFactory(contract);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddFactory<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderFactory<TContract>(contract, builder);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddSingleton<TContract>() where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new ActivatorSingleton(contract);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddSingleton<TContract>(TContract instance) where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new InstanceSingleton(contract, instance);

            return AddDependency(dependency);
        }

        public DependencyBuilder AddSingleton<TContract>(Func<DependencyContainer, TContract> builder)
            where TContract : class
        {
            var contract = Typeof<TContract>.Raw;
            var dependency = new BuilderSingleton<TContract>(contract, builder);

            return AddDependency(dependency);
        }

        public DependencyContainer Build()
        {
            return new DependencyContainer(_dependencies);
        }
    }
}