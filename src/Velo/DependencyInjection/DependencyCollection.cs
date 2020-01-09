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
            switch (lifetime)
            {
                case DependencyLifetime.Scope:
                    var scopeResolver = new CompiledResolver(implementation, _engine);
                    return AddDependency(new ScopeDependency(contracts, scopeResolver));
                case DependencyLifetime.Singleton:
                    var singletonResolver = new ActivatorResolver(implementation);
                    return AddDependency(new SingletonDependency(contracts, singletonResolver));
                case DependencyLifetime.Transient:
                    var transientResolver = new CompiledResolver(implementation, _engine);
                    return AddDependency(new TransientDependency(contracts, transientResolver));
            }

            throw Error.InvalidDependencyLifetime();
        }

        public DependencyCollection AddDependency<TContract>(Func<IDependencyScope, TContract> builder,
            DependencyLifetime lifetime)
            where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            var resolver = new DelegateResolver<TContract>(builder);

            switch (lifetime)
            {
                case DependencyLifetime.Scope:
                    return AddDependency(new ScopeDependency(contracts, resolver));
                case DependencyLifetime.Singleton:
                    return AddDependency(new SingletonDependency(contracts, resolver));
                case DependencyLifetime.Transient:
                    return AddDependency(new TransientDependency(contracts, resolver));
            }

            throw Error.InvalidDependencyLifetime();
        }

        public DependencyCollection AddFactory(IDependencyFactory factory)
        {
            _engine.AddFactory(factory);
            return this;
        }

        public DependencyCollection AddInstance<TContract>(TContract instance)
            where TContract : class
        {
            var resolver = new InstanceResolver(instance);
            return AddDependency(new ScopeDependency(Typeof<TContract>.Raw, resolver));
        }

        #region AddScoped

        public DependencyCollection AddScoped(Type implementation)
        {
            return AddDependency(implementation, implementation, DependencyLifetime.Scope);
        }

        public DependencyCollection AddScoped(Type contract, Type implementation)
        {
            return AddDependency(contract, implementation, DependencyLifetime.Scope);
        }

        public DependencyCollection AddScoped<TImplementation>()
        {
            var implementation = Typeof<TImplementation>.Raw;
            return AddDependency(implementation, implementation, DependencyLifetime.Scope);
        }

        public DependencyCollection AddScoped<TContract>(Func<IDependencyScope, TContract> builder)
            where TContract : class
        {
            return AddDependency(builder, DependencyLifetime.Scope);
        }

        public DependencyCollection AddScoped<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return AddDependency(contract, implementation, DependencyLifetime.Scope);
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
            return new DependencyProvider(_engine);
        }

        public bool Contains<TContract>() => _engine.Contains(Typeof<TContract>.Raw);
        
        public bool Contains(Type type) => _engine.Contains(type);

        public DependencyCollection Scan(Action<DependencyScanner> action)
        {
            var scanner = new DependencyScanner();

            action(scanner);
            scanner.Execute(this);

            return this;
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