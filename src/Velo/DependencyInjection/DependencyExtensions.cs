using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Velo.Collections.Local;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public static class DependencyExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Activate<T>(this IDependencyScope scope, ConstructorInfo? constructor = null)
        {
            return (T) scope.Activate(Typeof<T>.Raw, constructor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DependencyCollection AddInstance(
            this DependencyCollection dependencies,
            Type contract,
            object instance)
        {
            return dependencies.AddInstance(new[] {contract}, instance);
        }

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
            Func<IDependencyScope, TContract> builder)
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

        public static DependencyCollection AddSingleton(
            this DependencyCollection dependencies, Type implementation)
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
            Func<IDependencyScope, TContract> builder)
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
            Func<IDependencyScope, TContract> builder)
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

        public static DependencyLifetime DefineLifetime(this IDependency[] dependencies)
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
        public static DependencyLifetime GetLifetime<TContract>(this DependencyCollection dependencies)
            where TContract : class
        {
            return dependencies.GetLifetime(Typeof<TContract>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequiredService<T>(this IDependencyScope scope) where T : class
        {
            return (T) scope.GetRequiredService(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetService<T>(this IDependencyScope scope) where T : class
        {
            return (T?) scope.GetService(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[]? GetServices<T>(this IDependencyScope scope) where T : class
        {
            return (T[]?) scope.GetService(Typeof<T[]>.Raw);
        }
    }
}