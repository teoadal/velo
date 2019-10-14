using System;
using System.Runtime.CompilerServices;
using Velo.Collections;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;

namespace Velo.ECS.Systems
{
    internal sealed class SystemAllover : DependencyAllover
    {
        private readonly Type _beginUpdateSystem;
        private readonly Type _endUpdateSystem;
        private readonly Type _initializeSystem;
        private readonly Type _updateSystem;

        public SystemAllover()
        {
            _beginUpdateSystem = typeof(IBeginUpdateSystem);
            _endUpdateSystem = typeof(IEndUpdateSystem);
            _initializeSystem = typeof(IInitializeSystem);
            _updateSystem = typeof(IUpdateSystem);
        }

        public override void TryRegister(DependencyCollection collection, Type implementation)
        {
            var contracts = new LocalVector<Type>();

            TryAdd(ref contracts, _beginUpdateSystem, implementation);
            TryAdd(ref contracts, _endUpdateSystem, implementation);
            TryAdd(ref contracts, _initializeSystem, implementation);
            TryAdd(ref contracts, _updateSystem, implementation);

            collection.AddScoped(contracts.ToArray(), implementation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryAdd(ref LocalVector<Type> contracts, Type contract, Type implementation)
        {
            if (implementation.IsAssignableFrom(contract))
            {
                contracts.Add(contract);
            }
        }
    }
}