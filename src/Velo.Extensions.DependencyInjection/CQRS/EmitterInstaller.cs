using System;
using Velo.CQRS;
using Velo.CQRS.Pipeline;
using Velo.DependencyInjection;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmitterInstaller
    {
        public static IServiceCollection AddEmitter(this IServiceCollection services)
        {
            services.AddScoped<IEmitter>(provider => new Emitter(provider));
            return services;
        }

        public static IServiceCollection AddCommandBehaviour<TBehaviour>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TBehaviour);
            var processorInterfaces = PipelineTypes.CommandBehaviourTypes;

            var descriptor = BuildProcessorDescriptor(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            TryAddCommandPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddCommandProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorInterfaces = PipelineTypes.CommandProcessorTypes;

            var descriptor = BuildProcessorDescriptor(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            TryAddCommandPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }
        
        public static IServiceCollection AddQueryBehaviour<TBehaviour>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TBehaviour);
            var processorInterfaces = PipelineTypes.QueryBehaviourTypes;

            var descriptor = BuildProcessorDescriptor(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            TryAddQueryPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddQueryProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorInterfaces = PipelineTypes.QueryProcessorTypes;

            var descriptor = BuildProcessorDescriptor(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            TryAddQueryPipeline(services, descriptor.ServiceType, lifetime);

            return services;
        }

        public static IServiceCollection AddNotificationProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorInterfaces = PipelineTypes.NotificationProcessorTypes;

            var descriptor = BuildProcessorDescriptor(implementation, processorInterfaces, lifetime);
            services.Add(descriptor);

            var notificationType = descriptor.ServiceType.GenericTypeArguments[0];
            var pipelineType = PipelineTypes.Notification.MakeGenericType(notificationType);

            if (NeedCreatePipeline(services, pipelineType, lifetime))
            {
                services.Add(new ServiceDescriptor(
                    pipelineType,
                    PipelineActivators.GetNotificationPipelineActivator(notificationType),
                    lifetime));
            }

            return services;
        }

        private static ServiceDescriptor BuildProcessorDescriptor(Type implementation, Type[] processorInterfaces,
            ServiceLifetime lifetime)
        {
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, processorInterfaces);

            if (contracts.Length == 0) throw NotImplementedProcessorType(implementation);
            if (contracts.Length > 1) throw ManyImplementedProcessorType(implementation);

            return new ServiceDescriptor(contracts[0], implementation, lifetime);
        }

        private static bool NeedCreatePipeline(IServiceCollection services, Type pipelineType, ServiceLifetime lifetime)
        {
            foreach (var service in services)
            {
                if (service.ServiceType != pipelineType) continue;

                if (service.Lifetime > lifetime) return false;
                services.Remove(service);
                break;
            }

            return true;
        }

        private static void TryAddCommandPipeline(IServiceCollection services, Type contract, ServiceLifetime lifetime)
        {
            var commandType = contract.GenericTypeArguments[0];
            var pipelineType = PipelineTypes.Command.MakeGenericType(commandType);

            if (NeedCreatePipeline(services, pipelineType, lifetime))
            {
                services.Add(new ServiceDescriptor(
                    pipelineType,
                    PipelineActivators.GetCommandPipelineActivator(commandType),
                    lifetime));
            }
        }
        
        private static void TryAddQueryPipeline(IServiceCollection services, Type contract, ServiceLifetime lifetime)
        {
            var queryType = contract.GenericTypeArguments[0];
            var resultType = contract.GenericTypeArguments[1];
            var pipelineType = PipelineTypes.Query.MakeGenericType(queryType, resultType);

            if (NeedCreatePipeline(services, pipelineType, lifetime))
            {
                services.Add(new ServiceDescriptor(
                    pipelineType,
                    PipelineActivators.GetQueryPipelineActivator(queryType, resultType),
                    lifetime));
            }
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