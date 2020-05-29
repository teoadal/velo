using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Collections.Local;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public static class DependencyExtensions
    {
        #region Activate

        public static object Activate(
            this IServiceProvider services,
            Type implementation,
            ConstructorInfo? constructor = null)
        {
            EnsureCanActivated(implementation);

            constructor ??= ReflectionUtils.GetConstructor(implementation);

            if (constructor == null) throw Error.DefaultConstructorNotFound(implementation);

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return constructor.Invoke(Array.Empty<object>());
            }

            try
            {
                var parameterValues = new object?[parameters.Length];
                for (var i = parameters.Length - 1; i >= 0; i--)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;

                    parameterValues[i] = !parameter.HasDefaultValue // required
                        ? GetRequired(services, parameterType)
                        : services.GetService(parameterType);
                }

                return ReflectionUtils.TryInvoke(constructor, parameterValues);
            }
            catch (KeyNotFoundException e)
            {
                throw Error.DependencyNotRegistered($"{e.Message} <- '{ReflectionUtils.GetName(implementation)}'");
            }
        }

        public static object Activate(
            this IServiceProvider services,
            Type implementation,
            LocalList<object> possibleInjections)
        {
            EnsureCanActivated(implementation);

            var constructor = ReflectionUtils.GetConstructor(implementation);

            if (constructor == null) throw Error.DefaultConstructorNotFound(implementation);

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return constructor.Invoke(Array.Empty<object>());
            }

            var injections = possibleInjections.Select(injection => (injection.GetType(), injection));

            var parameterValues = new object?[parameters.Length];
            for (var i = parameters.Length - 1; i >= 0; i--)
            {
                var parameter = parameters[i];
                var parameterType = parameter.ParameterType;

                object? parameterValue = null;
                foreach (var (injectionType, injectionValue) in injections)
                {
                    if (!parameterType.IsAssignableFrom(injectionType)) continue;

                    parameterValue = injectionValue;
                    break;
                }

                if (parameterValue != null) parameterValues[i] = parameterValue;
                else
                {
                    parameterValues[i] = !parameter.HasDefaultValue // required
                        ? GetRequired(services, parameterType)
                        : services.GetService(parameterType);
                }
            }

            return ReflectionUtils.TryInvoke(constructor, parameterValues);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Activate<T>(this IServiceProvider services, ConstructorInfo? constructor = null)
        {
            return (T) Activate(services, Typeof<T>.Raw, constructor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Activate<T>(this IServiceProvider services, LocalList<object> possibleInjections)
        {
            return (T) Activate(services, typeof(T), possibleInjections);
        }

        #endregion

        #region AddInstance

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddInstance(
            this DependencyCollection dependencies,
            Type contract,
            object instance)
        {
            return dependencies.AddInstance(new[] {contract}, instance);
        }

        public static DependencyCollection AddInstance<TContract>(
            this DependencyCollection dependencies,
            TContract instance)
            where TContract : class
        {
            var contracts = new[] {Typeof<TContract>.Raw};
            return dependencies.Add(new InstanceDependency(contracts, instance));
        }

        #endregion

        internal static DependencyCollection AddFactory<TContract, TImplementation>(
            this DependencyCollection dependencies,
            Func<DependencyFactoryBuilder, DependencyFactoryBuilder> buildAction)
            where TImplementation : class, TContract
        {
            var builder = buildAction(new DependencyFactoryBuilder(typeof(TContract), typeof(TImplementation)));
            dependencies.AddFactory(builder.Build());

            return dependencies;
        }

        #region AddScoped

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddScoped(
            this DependencyCollection dependencies,
            Type implementation)
        {
            return dependencies.AddDependency(implementation, implementation, DependencyLifetime.Scoped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddScoped(
            this DependencyCollection dependencies,
            Type contract,
            Type implementation)
        {
            return dependencies.AddDependency(contract, implementation, DependencyLifetime.Scoped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddScoped<TImplementation>(this DependencyCollection dependencies)
        {
            var implementation = Typeof<TImplementation>.Raw;
            return dependencies.AddDependency(implementation, implementation, DependencyLifetime.Scoped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddScoped<TContract>(
            this DependencyCollection dependencies,
            Func<IServiceProvider, TContract> builder)
            where TContract : class
        {
            var contracts = new[] {typeof(TContract)};
            return dependencies.AddDependency(contracts, builder, DependencyLifetime.Scoped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddScoped<TContract, TImplementation>(this DependencyCollection dependencies)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return dependencies.AddDependency(contract, implementation, DependencyLifetime.Scoped);
        }

        #endregion

        #region AddSingleton

        public static DependencyCollection AddSingleton(this DependencyCollection dependencies, Type implementation)
        {
            if (implementation.IsGenericTypeDefinition)
            {
                var factory = new GenericFactory(implementation, implementation, DependencyLifetime.Singleton);
                dependencies.AddFactory(factory);
            }
            else
            {
                dependencies.AddDependency(implementation, implementation, DependencyLifetime.Singleton);
            }

            return dependencies;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddSingleton(
            this DependencyCollection dependencies,
            Type contract,
            Type implementation)
        {
            return dependencies.AddDependency(contract, implementation, DependencyLifetime.Singleton);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddSingleton<TImplementation>(this DependencyCollection dependencies)
        {
            var implementation = Typeof<TImplementation>.Raw;
            return dependencies.AddDependency(implementation, implementation, DependencyLifetime.Singleton);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddSingleton<TContract>(
            this DependencyCollection dependencies,
            Func<IServiceProvider, TContract> builder)
            where TContract : class
        {
            var contracts = new[] {typeof(TContract)};
            return dependencies.AddDependency(contracts, builder, DependencyLifetime.Singleton);
        }

        public static DependencyCollection AddSingleton<TContract, TImplementation>(
            this DependencyCollection dependencies)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return dependencies.AddDependency(contract, implementation, DependencyLifetime.Singleton);
        }

        #endregion

        #region AddTransient

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddTransient(
            this DependencyCollection dependencies,
            Type implementation)
        {
            return dependencies.AddDependency(implementation, implementation, DependencyLifetime.Transient);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddTransient(
            this DependencyCollection dependencies,
            Type contract,
            Type implementation)
        {
            return dependencies.AddDependency(contract, implementation, DependencyLifetime.Transient);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddTransient<TImplementation>(this DependencyCollection dependencies)
        {
            var implementation = Typeof<TImplementation>.Raw;
            return dependencies.AddDependency(implementation, implementation, DependencyLifetime.Transient);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddTransient<TContract>(
            this DependencyCollection dependencies,
            Func<IServiceProvider, TContract> builder)
            where TContract : class
        {
            var contracts = new[] {typeof(TContract)};
            return dependencies.AddDependency(contracts, builder, DependencyLifetime.Transient);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddTransient<TContract, TImplementation>(
            this DependencyCollection dependencies)
            where TImplementation : TContract
        {
            var contract = Typeof<TContract>.Raw;
            var implementation = typeof(TImplementation);

            return dependencies.AddDependency(contract, implementation, DependencyLifetime.Transient);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains<TContract>(this DependencyCollection dependencies)
            where TContract : class
        {
            return dependencies.Contains(Typeof<TContract>.Raw);
        }

        #region DefineLifetime

        public static DependencyLifetime DefineLifetime(this LocalList<DependencyLifetime> lifetimes)
        {
            int singleton = 0, transient = 0;
            foreach (var lifetime in lifetimes)
            {
                switch (lifetime)
                {
                    case DependencyLifetime.Singleton:
                        singleton++;
                        break;
                    case DependencyLifetime.Transient:
                        transient++;
                        break;
                }
            }

            if (singleton == lifetimes.Length) return DependencyLifetime.Singleton;
            return transient == 0
                ? DependencyLifetime.Scoped
                : DependencyLifetime.Transient;
        }

        public static DependencyLifetime DefineLifetime(this IDependency?[] dependencies)
        {
            var lifetimes = new LocalList<DependencyLifetime>();

            foreach (var dependency in dependencies)
            {
                if (dependency == null) continue;
                lifetimes.Add(dependency.Lifetime);
            }

            return DefineLifetime(lifetimes);
        }

        public static DependencyLifetime DefineLifetime(this IDependencyEngine engine, Type implementation)
        {
            var constructor = ReflectionUtils.GetConstructor(implementation);

            if (constructor == null) throw Error.DefaultConstructorNotFound(implementation);

            var parameters = constructor.GetParameters();

            var dependencies = new LocalList<DependencyLifetime>();
            foreach (var parameter in parameters)
            {
                var required = !parameter.HasDefaultValue;
                var dependency = required
                    ? engine.GetRequiredDependency(parameter.ParameterType)
                    : engine.GetDependency(parameter.ParameterType);

                if (dependency != null)
                {
                    dependencies.Add(dependency.Lifetime);
                }
            }

            return dependencies.DefineLifetime();
        }

        #endregion

        #region Get service

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? Get(this IServiceProvider services, Type contract)
        {
            return services.GetService(contract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Get<T>(this IServiceProvider services) where T : class
        {
            return (T?) services.GetService(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[]? GetArray<T>(this IServiceProvider services)
        {
            return (T[]?) services.GetService(Typeof<T[]>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetRequired(this IServiceProvider services, Type contract)
        {
            return services.GetService(contract) ?? throw Error.DependencyNotRegistered(contract);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequired<T>(this IServiceProvider services) where T : class
        {
            return (T) services.GetService(Typeof<T>.Raw) ?? throw Error.DependencyNotRegistered(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetRequiredArray<T>(this IServiceProvider services)
        {
            var contract = Typeof<T[]>.Raw;
            return (T[]) services.GetService(contract) ?? throw Error.DependencyNotRegistered(contract);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDependency[] GetApplicable<TContract>(this DependencyCollection dependencies)
            where TContract : class
        {
            return dependencies.GetApplicable(Typeof<TContract>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDependency? GetDependency<TContract>(this DependencyCollection dependencies)
            where TContract : class
        {
            return dependencies.GetDependency(Typeof<TContract>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDependency GetRequiredDependency<TContract>(this DependencyCollection dependencies)
            where TContract : class
        {
            return dependencies.GetRequiredDependency(Typeof<TContract>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyLifetime GetLifetime(this DependencyCollection dependencies, Type contract)
        {
            return dependencies.GetRequiredDependency(contract).Lifetime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyLifetime GetLifetime<TContract>(this DependencyCollection dependencies)
            where TContract : class
        {
            return dependencies.GetRequiredDependency(Typeof<TContract>.Raw).Lifetime;
        }

        #region Register collector

        public static DependencyScanner RegisterAsScoped(this DependencyScanner scanner, Type contract)
        {
            return scanner.Register(contract, DependencyLifetime.Scoped);
        }

        public static DependencyScanner RegisterAsScoped<TContract>(this DependencyScanner scanner)
            where TContract : class
        {
            return scanner.Register(Typeof<TContract>.Raw, DependencyLifetime.Scoped);
        }

        public static DependencyScanner RegisterAsSingleton(this DependencyScanner scanner, Type contract)
        {
            return scanner.Register(contract, DependencyLifetime.Singleton);
        }

        public static DependencyScanner RegisterAsSingleton<TContract>(this DependencyScanner scanner)
            where TContract : class
        {
            return scanner.Register(Typeof<TContract>.Raw, DependencyLifetime.Singleton);
        }

        public static DependencyScanner RegisterAsTransient(this DependencyScanner scanner, Type contract)
        {
            return scanner.Register(contract, DependencyLifetime.Transient);
        }

        public static DependencyScanner RegisterAsTransient<TContract>(this DependencyScanner scanner)
            where TContract : class
        {
            return scanner.Register(Typeof<TContract>.Raw, DependencyLifetime.Transient);
        }

        #endregion

        private static void EnsureCanActivated(Type implementation)
        {
            if (implementation.IsInterface || implementation.IsGenericTypeDefinition)
            {
                throw Error.InvalidOperation($"Type {ReflectionUtils.GetName(implementation)} can't be activated");
            }
        }
    }
}