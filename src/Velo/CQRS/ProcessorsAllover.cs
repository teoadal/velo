using System;
using System.Linq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.CQRS
{
    internal sealed class ProcessorsAllover : IDependencyAllover
    {
        private readonly DependencyLifetime _lifetime;
        private readonly Type[] _processorTypes;

        public ProcessorsAllover(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;

            _processorTypes = Types.CommandProcessorTypes
                .Concat(Types.CommandBehaviourTypes)
                .Concat(Types.NotificationProcessorTypes)
                .Concat(Types.QueryProcessorTypes)
                .Concat(Types.QueryBehaviourTypes)
                .ToArray();
        }

        public void TryRegister(DependencyCollection collection, Type implementation)
        {
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, _processorTypes);

            if (contracts.Length == 0) return;

            if (implementation.IsGenericTypeDefinition)
            {
                var contract = contracts[0];
                collection.AddFactory(new GenericFactory(contract, implementation, _lifetime));
            }
            else
            {
                contracts.Add(implementation);
                collection.AddDependency(contracts.ToArray(), implementation, _lifetime);
            }
        }
    }
}