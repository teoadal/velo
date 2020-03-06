using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.DependencyInjection;
using Velo.Utils;

namespace Velo.Extensions.DependencyInjection.CQRS
{
    internal static class ProcessorDescriptor
    {
        public static ServiceDescriptor Build(Type implementation, Type[] processorInterfaces,
            ServiceLifetime lifetime)
        {
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, processorInterfaces);

            if (contracts.Length == 0) throw NotImplementedProcessorType(implementation);
            if (contracts.Length > 1) throw ManyImplementedProcessorType(implementation);

            return new ServiceDescriptor(contracts[0], implementation, lifetime);
        }

        public static ServiceDescriptor Build(object instance, Type[] processorInterfaces)
        {
            var implementation = instance.GetType();
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, processorInterfaces);

            if (contracts.Length == 0) throw NotImplementedProcessorType(implementation);
            if (contracts.Length > 1) throw ManyImplementedProcessorType(implementation);

            return new ServiceDescriptor(contracts[0], instance);
        }
        
        private static InvalidOperationException NotImplementedProcessorType(Type implementation)
        {
            var name = ReflectionUtils.GetName(implementation);
            return Error.InvalidOperation($"'{name}' must have implementation of processor interface");
        }

        private static InvalidOperationException ManyImplementedProcessorType(Type implementation)
        {
            var name = ReflectionUtils.GetName(implementation);
            var dependencyCollection = ReflectionUtils.GetName<DependencyCollection>();

            return Error.InvalidOperation(
                $"'{name}' has many implementation of processor interfaces: " +
                $"it's possible, if you using '{dependencyCollection}' in your project");
        }
    }
}