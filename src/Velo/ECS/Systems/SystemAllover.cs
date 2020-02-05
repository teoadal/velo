using System;
using System.Runtime.CompilerServices;
using Velo.Collections;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;

namespace Velo.ECS.Systems
{
    internal sealed class SystemAllover : IDependencyAllover
    {
        private readonly Type _beginUpdateSystem;
        private readonly Type _endUpdateSystem;
        private readonly Type _initializeSystem;
        private readonly Type _updateSystem;

        public SystemAllover()
        {
            _beginUpdateSystem = typeof(IBeforeUpdateSystem);
            _endUpdateSystem = typeof(IAfterUpdateSystem);
            _initializeSystem = typeof(IInitializeSystem);
            _updateSystem = typeof(IUpdateSystem);
        }

        public void TryRegister(DependencyCollection collection, Type implementation)
        {
            var contracts = new LocalList<Type>();

            TryAdd(ref contracts, _beginUpdateSystem, implementation);
            TryAdd(ref contracts, _endUpdateSystem, implementation);
            TryAdd(ref contracts, _initializeSystem, implementation);
            TryAdd(ref contracts, _updateSystem, implementation);

            collection.AddDependency(contracts.ToArray(), implementation, DependencyLifetime.Scoped);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryAdd(ref LocalList<Type> contracts, Type contract, Type implementation)
        {
            if (implementation.IsAssignableFrom(contract))
            {
                contracts.Add(contract);
            }
        }
    }
}