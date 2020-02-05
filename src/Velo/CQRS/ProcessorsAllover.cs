using System;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.CQRS
{
    internal sealed class ProcessorsAllover : IDependencyAllover
    {
        public static readonly Type[] CommandProcessorTypes =
        {
            typeof(ICommandPreProcessor<>),
            typeof(ICommandProcessor<>),
            typeof(ICommandPostProcessor<>)
        };

        public static readonly Type[] NotificationProcessorTypes =
        {
            typeof(INotificationProcessor<>)
        };

        public static readonly Type[] QueryProcessorTypes = {typeof(IQueryProcessor<,>)};

        private readonly DependencyLifetime _lifetime;
        private readonly ProcessorDescription[] _processorDescriptions;

        public ProcessorsAllover(DependencyLifetime lifetime)
        {
            _lifetime = lifetime;

            _processorDescriptions = new[]
            {
                new ProcessorDescription(typeof(ICommandProcessor), CommandProcessorTypes),
                new ProcessorDescription(typeof(INotificationProcessor), NotificationProcessorTypes),
                new ProcessorDescription(typeof(IQueryProcessor), QueryProcessorTypes)
            };
        }

        public void TryRegister(DependencyCollection collection, Type implementation)
        {
            Type[] genericInterfaces = null;

            foreach (var description in _processorDescriptions)
            {
                if (!description.BaseType.IsAssignableFrom(implementation)) continue;

                genericInterfaces = description.ProcessorTypes;
                break;
            }

            if (genericInterfaces == null) return;

            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, genericInterfaces);

            if (implementation.IsGenericTypeDefinition)
            {
                var contract = contracts[0];
                collection.AddFactory(new GenericFactory(contract, implementation, _lifetime));
            }
            else
            {
                collection.AddDependency(contracts.ToArray(), implementation, _lifetime);
            }
        }

        private readonly struct ProcessorDescription
        {
            public readonly Type BaseType;
            public readonly Type[] ProcessorTypes;

            public ProcessorDescription(Type baseType, Type[] processorTypes)
            {
                BaseType = baseType;
                ProcessorTypes = processorTypes;
            }
        }
    }
}