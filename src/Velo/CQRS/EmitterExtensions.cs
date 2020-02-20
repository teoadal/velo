using System;
using System.Runtime.CompilerServices;
using Velo.CQRS.Commands;
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

        public static DependencyCollection AddCommandBehaviour<TBehaviour>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var behaviourTypes = PipelineTypes.CommandBehaviourTypes;
            if (!AddDependencies(collection, typeof(TBehaviour), behaviourTypes, lifetime))
            {
                throw Error.InvalidOperation($"'{ReflectionUtils.GetName<TBehaviour>()}' not implemented command behaviour interface");
            }

            return collection;
        }

        public static DependencyCollection AddCommandProcessor<TCommand>(this DependencyCollection collection, Action<TCommand> processor)
            where TCommand: ICommand
        {
            return collection.AddInstance(new ActionCommandProcessor<TCommand>(processor));
        }
        
        public static DependencyCollection AddCommandProcessor<TCommand, TContext>(
            this DependencyCollection collection, Action<TCommand, TContext> processor)
            where TCommand: ICommand
        {
            return collection.AddSingleton<ICommandProcessor<TCommand>>(ctx => 
                new ActionCommandProcessor<TCommand, TContext>(processor, ctx.GetRequiredService<DependencyProvider>()));
        }
        
        public static DependencyCollection AddCommandProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var processorTypes = PipelineTypes.CommandProcessorTypes;
            if (!AddDependencies(collection, typeof(TProcessor), processorTypes, lifetime))
            {
                throw Error.InvalidOperation($"'{ReflectionUtils.GetName<TProcessor>()}' not implemented any of command processors interface");
            }

            return collection;
        }

        public static DependencyCollection AddNotificationProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var processorTypes = PipelineTypes.NotificationProcessorTypes;
            if (!AddDependencies(collection, typeof(TProcessor), processorTypes, lifetime))
            {
                throw Error.InvalidOperation($"'{ReflectionUtils.GetName<TProcessor>()}' not implemented notification processor interface");
            }

            return collection;
        }

        public static DependencyCollection AddQueryBehaviour<TBehaviour>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var behaviourTypes = PipelineTypes.QueryBehaviourTypes;
            if (!AddDependencies(collection, typeof(TBehaviour), behaviourTypes, lifetime))
            {
                throw Error.InvalidOperation($"'{ReflectionUtils.GetName<TBehaviour>()}' not implemented query behaviour interface");
            }

            return collection;
        }

        public static DependencyCollection AddQueryProcessor<TQuery, TResult>(this DependencyCollection collection,
            Func<TQuery, TResult> processor)
            where TQuery: IQuery<TResult>
        {
            return collection.AddInstance(new ActionQueryProcessor<TQuery, TResult>(processor));
        }
        
        public static DependencyCollection AddQueryProcessor<TQuery, TContext, TResult>(
            this DependencyCollection collection,
            Func<TQuery, TContext, TResult> processor)
            where TQuery: IQuery<TResult>
        {
            return collection.AddSingleton<IQueryProcessor<TQuery, TResult>>(ctx => 
                new ActionQueryProcessor<TQuery, TContext, TResult>(processor, ctx.GetRequiredService<DependencyProvider>()));
        }
        
        public static DependencyCollection AddQueryProcessor<TProcessor>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var processorTypes = PipelineTypes.QueryProcessorTypes;
            if (!AddDependencies(collection, typeof(TProcessor), processorTypes, lifetime))
            {
                throw Error.InvalidOperation($"'{ReflectionUtils.GetName<TProcessor>()}' not implemented any of query processors interface");
            }

            return collection;
        }

        public static DependencyScanner AddEmitterProcessors(this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            return scanner.UseAllover(new ProcessorsAllover(lifetime));
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
    }
}