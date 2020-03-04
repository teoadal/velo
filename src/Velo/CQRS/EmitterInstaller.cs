using System;
using System.Runtime.CompilerServices;
using Velo.CQRS;
using Velo.CQRS.Commands;
using Velo.CQRS.Pipeline;
using Velo.CQRS.Queries;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class EmitterInstaller
    {
        public static DependencyCollection AddEmitter(this DependencyCollection collection)
        {
            collection
                .AddFactory(new PipelineFactory(PipelineTypes.Command))
                .AddFactory(new PipelineFactory(PipelineTypes.Notification))
                .AddFactory(new PipelineFactory(PipelineTypes.Query))
                .AddScoped<IEmitter>(ctx => new Emitter(ctx));

            return collection;
        }

        public static DependencyCollection AddCommandBehaviour<TBehaviour>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var behaviourTypes = PipelineTypes.CommandBehaviourTypes;
            var implementation = typeof(TBehaviour);
            
            if (!AddDependencies(collection, implementation, behaviourTypes, lifetime))
            {
                throw NotImplementedCommand(implementation, "behaviour");
            }

            return collection;
        }

        public static DependencyCollection AddCommandProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorTypes = PipelineTypes.CommandProcessorTypes;
            
            if (!AddDependencies(collection, implementation, processorTypes, lifetime))
            {
                throw NotImplementedCommand(implementation, "processor");
            }

            return collection;
        }

        public static DependencyCollection AddCommandProcessor(this DependencyCollection collection, object processor)
        {
            var implementation = processor.GetType();
            var processorTypes = PipelineTypes.CommandProcessorTypes;

            if (!AddInstance(collection, processor, processorTypes))
            {
                throw NotImplementedCommand(implementation, "processor");
            }
            
            return collection;
        }
        
        public static DependencyCollection AddNotificationProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorTypes = PipelineTypes.NotificationProcessorTypes;
            
            if (!AddDependencies(collection, implementation, processorTypes, lifetime))
            {
                throw NotImplementedNotification(implementation);
            }

            return collection;
        }

        public static DependencyCollection AddNotificationProcessor(this DependencyCollection collection, object processor)
        {
            var implementation = processor.GetType();
            var processorTypes = PipelineTypes.NotificationProcessorTypes;

            if (!AddInstance(collection, processor, processorTypes))
            {
                throw NotImplementedNotification(implementation);
            }
            
            return collection;
        }
        
        public static DependencyCollection AddQueryBehaviour<TBehaviour>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var behaviourTypes = PipelineTypes.QueryBehaviourTypes;
            var implementation = typeof(TBehaviour);
            
            if (!AddDependencies(collection, implementation, behaviourTypes, lifetime))
            {
                throw NotImplementedQuery(implementation, "behaviour");
            }

            return collection;
        }

        public static DependencyCollection AddQueryProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorTypes = PipelineTypes.QueryProcessorTypes;
            
            if (!AddDependencies(collection, implementation, processorTypes, lifetime))
            {
                throw NotImplementedQuery(implementation, "processor");
            }

            return collection;
        }

        public static DependencyCollection AddQueryProcessor(this DependencyCollection collection, object processor)
        {
            var implementation = processor.GetType();
            var processorTypes = PipelineTypes.QueryProcessorTypes;

            if (!AddInstance(collection, processor, processorTypes))
            {
                throw NotImplementedQuery(implementation, "processor");
            }
            
            return collection;
        }
        
        public static DependencyScanner AddEmitterProcessors(this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            return scanner.UseAllover(new ProcessorsAllover(lifetime));
        }

        public static DependencyCollection CreateProcessor<TCommand>(this DependencyCollection collection,
            Action<TCommand> processor)
            where TCommand : ICommand
        {
            return collection.AddInstance(new ActionCommandProcessor<TCommand>(processor));
        }

        public static DependencyCollection CreateProcessor<TCommand, TContext>(
            this DependencyCollection collection, Action<TCommand, TContext> processor)
            where TCommand : ICommand
        {
            return collection.AddSingleton<ICommandProcessor<TCommand>>(scope =>
                new ActionCommandProcessor<TCommand, TContext>(processor,
                    scope.GetRequiredService<DependencyProvider>()));
        }

        public static DependencyCollection CreateProcessor<TQuery, TResult>(this DependencyCollection collection,
            Func<TQuery, TResult> processor)
            where TQuery : IQuery<TResult>
        {
            return collection.AddInstance(new ActionQueryProcessor<TQuery, TResult>(processor));
        }

        public static DependencyCollection CreateProcessor<TQuery, TContext, TResult>(
            this DependencyCollection collection, Func<TQuery, TContext, TResult> processor)
            where TQuery : IQuery<TResult>
        {
            return collection.AddSingleton<IQueryProcessor<TQuery, TResult>>(scope =>
                new ActionQueryProcessor<TQuery, TContext, TResult>(processor,
                    scope.GetRequiredService<DependencyProvider>()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AddDependencies(
            DependencyCollection collection,
            Type implementation,
            Type[] genericInterfaces,
            DependencyLifetime lifetime)
        {
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, genericInterfaces);

            if (contracts.Length == 0)
            {
                return false;
            }

            contracts.Add(implementation);
            collection.AddDependency(contracts.ToArray(), implementation, lifetime);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AddInstance(
            DependencyCollection collection,
            object instance,
            Type[] genericInterfaces)
        {
            var type = instance.GetType();
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(type, genericInterfaces);

            if (contracts.Length == 0)
            {
                return false;
            }
            
            collection.AddInstance(contracts.ToArray(), instance);
            return true;
        }
        
        private static InvalidOperationException NotImplementedCommand(Type type, string pipelinePart)
        {
            var typeName = ReflectionUtils.GetName(type);
            return Error.InvalidOperation($"'{typeName}' not implemented any of command {pipelinePart} interface");
        }
        
        private static InvalidOperationException NotImplementedNotification(Type type)
        {
            var typeName = ReflectionUtils.GetName(type);
            return Error.InvalidOperation($"'{typeName}' not implemented notification processor interface");
        }
        
        private static InvalidOperationException NotImplementedQuery(Type type, string pipelinePart)
        {
            var typeName = ReflectionUtils.GetName(type);
            return Error.InvalidOperation($"'{typeName}' not implemented any of query {pipelinePart} interface");
        }
    }
}