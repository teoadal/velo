using System;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.Extensions.DependencyInjection.CQRS.Commands;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandInstaller
    {
        public static IServiceCollection AddCommandBehaviour<TBehaviour>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TBehaviour);
            var processorInterfaces = Types.CommandBehaviourTypes;

            var descriptor = ProcessorDescriptor.Build(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddCommandBehaviour(this IServiceCollection services, object behaviour)
        {
            var processorInterfaces = Types.CommandProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(behaviour, processorInterfaces);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, ServiceLifetime.Singleton);

            return services;
        }

        public static IServiceCollection AddCommandProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorInterfaces = Types.CommandProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddCommandProcessor(this IServiceCollection services, object processor)
        {
            var processorInterfaces = Types.CommandProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(processor, processorInterfaces);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, ServiceLifetime.Singleton);

            return services;
        }

        private static void UpsertPipeline(IServiceCollection services, Type contract, ServiceLifetime lifetime)
        {
            var commandType = contract.GenericTypeArguments[0];
            var pipelineType = Types.CommandPipeline.MakeGenericType(commandType);

            if (services.RemoveLessLifetimeService(pipelineType, lifetime, true))
            {
                services.Add(new ServiceDescriptor(
                    pipelineType,
                    CommandPipelineFactory.GetActivator(commandType),
                    lifetime));
            }
        }
    }
}