using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.DependencyInjection.Engine;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyCollection
    {
        private readonly Dictionary<Type, DependencyDescription> _descriptions;
        private readonly List<ResolverFactory> _factories;

        public DependencyCollection()
        {
            _descriptions = new Dictionary<Type, DependencyDescription>(50);
            _factories = new List<ResolverFactory>(10);
        }

        #region Add

        public DependencyCollection Add(Type contract, DependencyResolver resolver)
        {
            Register(contract, resolver);

            return this;
        }

        public DependencyCollection Add(ResolverFactory resolverFactory)
        {
            Register(resolverFactory);

            return this;
        }

        public DependencyCollection Add(Type contract, Type implementation, DependencyLifetime lifetime)
        {
            switch (lifetime)
            {
                case DependencyLifetime.Scope:
                    AddScoped(contract, implementation);
                    break;
                case DependencyLifetime.Singleton:
                    AddSingleton(contract, implementation);
                    break;
                case DependencyLifetime.Transient:
                    AddTransient(contract, implementation);
                    break;
            }

            return this;
        }

        public DependencyCollection Add(Type[] contracts, Type implementation, DependencyLifetime lifetime)
        {
            switch (lifetime)
            {
                case DependencyLifetime.Scope:
                    AddScoped(contracts, implementation);
                    break;
                case DependencyLifetime.Singleton:
                    AddSingleton(contracts, implementation);
                    break;
                case DependencyLifetime.Transient:
                    AddTransient(contracts, implementation);
                    break;
            }

            return this;
        }
        
        #endregion

        public DependencyCollection AddInstance<TInstance>(TInstance instance)
            where TInstance : class
        {
            var resolver = new InstanceResolver(instance);

            Register(Typeof<TInstance>.Raw, resolver);

            return this;
        }

        #region AddGeneric

        public DependencyCollection AddGenericScoped(Type genericContract, Type genericImplementation = null)
        {
            Register(new GenericFactory(genericContract, genericImplementation, DependencyLifetime.Scope));

            return this;
        }

        public DependencyCollection AddGenericSingleton(Type genericContract, Type genericImplementation = null)
        {
            Register(new GenericFactory(genericContract, genericImplementation, DependencyLifetime.Singleton));

            return this;
        }

        public DependencyCollection AddGenericTransient(Type genericContract, Type genericImplementation = null)
        {
            Register(new GenericFactory(genericContract, genericImplementation, DependencyLifetime.Transient));

            return this;
        }

        #endregion

        #region AddScoped

        public DependencyCollection AddScoped<TImplementation>()
        {
            var contract = Typeof<TImplementation>.Raw;
            var resolver = new CompiledResolver(contract, DependencyLifetime.Scope);

            Register(contract, resolver);

            return this;
        }

        public DependencyCollection AddScoped<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var resolver = new CompiledResolver(typeof(TImplementation), DependencyLifetime.Scope);

            Register(Typeof<TContract>.Raw, resolver);

            return this;
        }

        public DependencyCollection AddScoped<TContract>(Func<DependencyProvider, TContract> builder)
            where TContract : class
        {
            var resolver = new DelegateResolver<TContract>(builder, DependencyLifetime.Scope);

            Register(Typeof<TContract>.Raw, resolver);

            return this;
        }

        public DependencyCollection AddScoped(Type contract, Type implementation)
        {
            var resolver = new CompiledResolver(implementation, DependencyLifetime.Scope);

            Register(contract, resolver);

            return this;
        }

        public DependencyCollection AddScoped(Type[] contracts, Type implementation)
        {
            var resolver = new CompiledResolver(implementation, DependencyLifetime.Scope);

            for (var i = 0; i < contracts.Length; i++)
            {
                Register(contracts[i], resolver);
            }

            return this;
        }

        #endregion

        #region AddSingleton

        public DependencyCollection AddSingleton<TImplementation>()
        {
            var contract = Typeof<TImplementation>.Raw;
            var resolver = new ActivatorResolver(contract, DependencyLifetime.Singleton);

            Register(contract, resolver);

            return this;
        }

        public DependencyCollection AddSingleton<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var resolver = new ActivatorResolver(typeof(TImplementation), DependencyLifetime.Singleton);
            Register(Typeof<TContract>.Raw, resolver);

            return this;
        }

        public DependencyCollection AddSingleton<TContract>(Func<DependencyProvider, TContract> builder)
            where TContract : class
        {
            var resolver = new DelegateResolver<TContract>(builder, DependencyLifetime.Singleton);

            Register(Typeof<TContract>.Raw, resolver);

            return this;
        }

        public DependencyCollection AddSingleton(Type contract, Type implementation)
        {
            var resolver = new ActivatorResolver(implementation, DependencyLifetime.Singleton);

            Register(contract, resolver);

            return this;
        }

        public DependencyCollection AddSingleton(Type[] contracts, Type implementation)
        {
            var resolver = new ActivatorResolver(implementation, DependencyLifetime.Singleton);

            for (var i = 0; i < contracts.Length; i++)
            {
                Register(contracts[i], resolver);
            }

            return this;
        }

        #endregion

        #region AddTransient

        public DependencyCollection AddTransient<TImplementation>()
        {
            var contract = Typeof<TImplementation>.Raw;
            var resolver = new CompiledResolver(contract, DependencyLifetime.Transient);

            Register(contract, resolver);

            return this;
        }

        public DependencyCollection AddTransient<TContract, TImplementation>()
            where TImplementation : TContract
        {
            var resolver = new CompiledResolver(typeof(TImplementation), DependencyLifetime.Transient);

            Register(Typeof<TContract>.Raw, resolver);

            return this;
        }

        public DependencyCollection AddTransient<TContract>(Func<DependencyProvider, TContract> builder)
            where TContract : class
        {
            var resolver = new DelegateResolver<TContract>(builder, DependencyLifetime.Transient);

            Register(Typeof<TContract>.Raw, resolver);

            return this;
        }

        public DependencyCollection AddTransient(Type contract, Type implementation)
        {
            var resolver = new CompiledResolver(implementation, DependencyLifetime.Transient);

            Register(contract, resolver);

            return this;
        }

        public DependencyCollection AddTransient(Type[] contracts, Type implementation)
        {
            var resolver = new CompiledResolver(implementation, DependencyLifetime.Transient);

            for (var i = 0; i < contracts.Length; i++)
            {
                Register(contracts[i], resolver);
            }

            return this;
        }

        #endregion

        public DependencyProvider BuildProvider()
        {
            using (var engineBuilder = new ConstructorEngine(_descriptions, _factories))
            {
                var engine = engineBuilder.Build();
                return new DependencyProvider(engine);
            }
        }

        public DependencyCollection Scan(Action<DependencyScanner> scannerConfiguration)
        {
            var scanner = new DependencyScanner();
            scannerConfiguration(scanner);
            scanner.Execute(this);

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Register(Type contract, DependencyResolver resolver)
        {
            var descriptions = _descriptions;
            if (descriptions.TryGetValue(contract, out var existsDescription))
            {
                descriptions[contract] = existsDescription.Add(resolver);
            }
            else
            {
                descriptions[contract] = new DependencyDescription(resolver);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Register(ResolverFactory factory)
        {
            _factories.Add(factory);
        }
    }
}