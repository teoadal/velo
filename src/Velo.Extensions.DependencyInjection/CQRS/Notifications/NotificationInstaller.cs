using System;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.Extensions.DependencyInjection.CQRS.Notifications;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class NotificationInstaller
    {
        public static IServiceCollection AddNotificationProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorInterfaces = Types.NotificationProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddNotificationProcessor(this IServiceCollection services, object processor)
        {
            var processorInterfaces = Types.NotificationProcessorTypes;

            var descriptor = ProcessorDescriptor.Build(processor, processorInterfaces);
            services.Add(descriptor);

            UpsertPipeline(services, descriptor.ServiceType, ServiceLifetime.Singleton);

            return services;
        }
        
        internal static void UpsertPipeline(IServiceCollection services, Type contract, ServiceLifetime lifetime)
        {
            var notificationType = contract.GenericTypeArguments[0];
            var pipelineType = Types.NotificationPipeline.MakeGenericType(notificationType);

            if (services.RemoveLessLifetimeService(pipelineType, lifetime, true))
            {
                services.Add(new ServiceDescriptor(
                    pipelineType,
                    NotificationPipelineFactory.GetActivator(notificationType),
                    lifetime));
            }
        }
    }
}