using System;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyCollection
    {
        private readonly DependencyEngine _engine;

        public DependencyCollection(int capacity = 64)
        {
            _engine = new DependencyEngine(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DependencyCollection AddDependency(IDependency dependency)
        {
            _engine.AddDependency(dependency);
            return this;
        }

        public DependencyCollection AddDependency(Type contract, Type implementation, DependencyLifetime lifetime)
        {
            if (contract.IsGenericTypeDefinition)
            {
                CheckIsGenericTypeDefinition(implementation);
                return AddFactory(new GenericFactory(contract, implementation, lifetime));
            }

            var contracts = new[] {contract};
            return AddDependency(contracts, implementation, lifetime);
        }

        public DependencyCollection AddDependency(Type[] contracts, Type implementation, DependencyLifetime lifetime)
        {
            var resolver = DependencyResolver.Build(lifetime, implementation, _engine);
            var dependency = Dependency.Build(lifetime, contracts, resolver);
            return AddDependency(dependency);
        }

        public DependencyCollection AddDependency<TContract>(Func<IDependencyScope, TContract> builder,
            DependencyLifetime lifetime)
            where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var resolver = new DelegateResolver<TContract>(builder);

            var dependency = Dependency.Build(lifetime, contracts, resolver);
            return AddDependency(dependency);
        }

        public DependencyCollection AddFactory(IDependencyFactory factory)
        {
            _engine.AddFactory(factory);
            return this;
        }

        public DependencyCollection AddInstance(Type[] contracts, object instance)
        {
            return AddDependency(new InstanceDependency(contracts, instance));
        }
        
        public DependencyCollection AddInstance(Type contract, object instance)
        {
            return AddInstance(new[] {contract}, instance);
        }
        
        public DependencyCollection AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            return AddInstance(Typeof<TContract>.Raw, instance);
        }

        #region AddScoped

        public DependencyCollection AddScoped(Type implementation)
        {
            return AddDependency(implementation, implementation, DependencyLifetime.Scoped);
        }

        public DependencyCollection AddScoped(Type contract, Type implementation)
        {
            return AddDependency(contract, implementation, DependencyLifetime.Scoped);
        }

        public DependencyCollection AddScoped<TImplementation>()
        {
            var implementation = Typeof<TImplementation>.Raw;
            return AddDependency(implementation, implementation, DependencyLifetime.Scoped);
        }

        public DependencyCollection AddScoped<TContract>(Func<IDependencyScope, TContract> builder)
            where TContract : class
        {
            return AddDependency(builder, DependencyLifetime.Scoped);
        }

        public DependencyCollection AddScoped<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return AddDependency(contract, implementation, DependencyLifetime.Scoped);
        }

        #endregion

        #region AddSingleton

        public DependencyCollection AddSingleton(Type implementation)
        {
            return implementation.IsGenericTypeDefinition
                ? AddFactory(new GenericFactory(implementation, implementation, DependencyLifetime.Singleton))
                : AddDependency(implementation, implementation, DependencyLifetime.Singleton);
        }

        public DependencyCollection AddSingleton(Type contract, Type implementation)
        {
            return AddDependency(contract, implementation, DependencyLifetime.Singleton);
        }

        public DependencyCollection AddSingleton<TImplementation>()
        {
            var implementation = Typeof<TImplementation>.Raw;
            return AddDependency(implementation, implementation, DependencyLifetime.Singleton);
        }

        public DependencyCollection AddSingleton<TContract>(Func<IDependencyScope, TContract> builder)
            where TContract : class
        {
            return AddDependency(builder, DependencyLifetime.Singleton);
        }

        public DependencyCollection AddSingleton<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return AddDependency(contract, implementation, DependencyLifetime.Singleton);
        }

        #endregion

        #region AddTransient

        public DependencyCollection AddTransient(Type implementation)
        {
            return AddDependency(implementation, implementation, DependencyLifetime.Transient);
        }

        public DependencyCollection AddTransient(Type contract, Type implementation)
        {
            return AddDependency(contract, implementation, DependencyLifetime.Transient);
        }

        public DependencyCollection AddTransient<TImplementation>()
        {
            var implementation = Typeof<TImplementation>.Raw;
            return AddDependency(implementation, implementation, DependencyLifetime.Transient);
        }

        public DependencyCollection AddTransient<TContract>(Func<IDependencyScope, TContract> builder)
            where TContract : class
        {
            return AddDependency(builder, DependencyLifetime.Transient);
        }

        public DependencyCollection AddTransient<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return AddDependency(contract, implementation, DependencyLifetime.Transient);
        }

        #endregion

        public DependencyProvider BuildProvider()
        {
            var provider = new DependencyProvider(_engine);

            var providerContracts = new[] {Typeof<DependencyProvider>.Raw, Typeof<IServiceProvider>.Raw};
            var providerDependency = new InstanceDependency(providerContracts, provider);

            _engine.AddDependency(providerDependency);

            return provider;
        }

        public bool Contains<TContract>() => _engine.Contains(Typeof<TContract>.Raw);

        public bool Contains(Type contract) => _engine.Contains(contract);

        public DependencyLifetime GetLifetime<TContract>() => _engine.GetDependency(Typeof<TContract>.Raw).Lifetime;
        
        public DependencyLifetime GetLifetime(Type contract) => _engine.GetDependency(contract).Lifetime;

        public DependencyCollection Scan(Action<DependencyScanner> action)
        {
            var scanner = new DependencyScanner(this);

            action(scanner);
            scanner.Execute();

            return this;
        }

        public bool Remove(Type contract)
        {
            return _engine.Remove(contract);
        }
        
        private static void CheckIsGenericTypeDefinition(Type type)
        {
            if (type != null && !type.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"{ReflectionUtils.GetName(type)} is not generic type definition");
            }
        }
    }
}