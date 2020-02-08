using System;
using System.Runtime.CompilerServices;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Pipeline;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.CQRS
{
    public static class EmitterExtensions
    {
        public static DependencyCollection AddEmitter(this DependencyCollection collection)
        {
            collection
                .AddFactory(new PipelineFactory(PipelineTypes.Command))
                .AddFactory(new PipelineFactory(PipelineTypes.Notification))
                .AddFactory(new PipelineFactory(PipelineTypes.Query))
                .AddScoped<Emitter>();

            return collection;
        }

        public static DependencyCollection AddCommandProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TProcessor : ICommandProcessor
        {
            var commandProcessorTypes = PipelineTypes.CommandProcessorTypes;
            AddDependencies(collection, typeof(TProcessor), commandProcessorTypes, lifetime);

            return collection;
        }

        public static DependencyCollection AddCommandBehaviour<TBehaviour>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var commandProcessorTypes = PipelineTypes.CommandBehaviourTypes;
            AddDependencies(collection, typeof(TBehaviour), commandProcessorTypes, lifetime);

            return collection;
        }

        public static DependencyCollection AddNotificationProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TProcessor : INotificationProcessor
        {
            var notificationProcessorTypes = PipelineTypes.NotificationProcessorTypes;
            AddDependencies(collection, typeof(TProcessor), notificationProcessorTypes, lifetime);

            return collection;
        }

        public static DependencyCollection AddQueryProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TProcessor : IQueryProcessor
        {
            var queryProcessorTypes = PipelineTypes.QueryProcessorTypes;
            AddDependencies(collection, typeof(TProcessor), queryProcessorTypes, lifetime);

            return collection;
        }

        public static DependencyScanner AddEmitterProcessors(this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            return scanner.UseAllover(new ProcessorsAllover(lifetime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddDependencies(
            DependencyCollection collection,
            Type implementation,
            Type[] genericInterfaces,
            DependencyLifetime lifetime)
        {
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, genericInterfaces);
            contracts.Add(implementation);

            collection.AddDependency(contracts.ToArray(), implementation, lifetime);
        }
    }
}