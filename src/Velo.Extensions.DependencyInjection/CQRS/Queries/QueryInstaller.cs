using System;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.Extensions.DependencyInjection.CQRS.Queries;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class QueryInstaller
    {
        public static IServiceCollection AddQueryBehaviour<TBehaviour>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TBehaviour);
            var processorInterfaces = Types.QueryBehaviourTypes;

            var descriptor = ProcessorDescriptor.Build(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddQueryBehaviour(this IServiceCollection services, object behaviour)
        {
            var processorInterfaces = Types.QueryBehaviourTypes;

            var descriptor = ProcessorDescriptor.Build(behaviour, processorInterfaces);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, ServiceLifetime.Singleton);

            return services;
        }
        
        public static IServiceCollection AddQueryProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorInterfaces = Types.QueryProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddQueryProcessor(this IServiceCollection services, object processor)
        {
            var processorInterfaces = Types.QueryProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(processor, processorInterfaces);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, ServiceLifetime.Singleton);

            return services;
        }
        
        private static void UpsertPipeline(IServiceCollection services, Type contract, ServiceLifetime lifetime)
        {
            var queryType = contract.GenericTypeArguments[0];
            var resultType = contract.GenericTypeArguments[1];
            var pipelineType = Types.QueryPipeline.MakeGenericType(queryType, resultType);

            if (services.RemoveLessLifetimeService(pipelineType, lifetime, true))
            {
                services.Add(new ServiceDescriptor(
                    pipelineType,
                    QueryPipelineFactory.GetActivator(queryType, resultType),
                    lifetime));
            }
        }
    }
}