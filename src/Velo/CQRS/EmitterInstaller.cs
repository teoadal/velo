using System;
using System.Runtime.CompilerServices;
using Velo.CQRS;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.CQRS.Notifications.Pipeline;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class EmitterInstaller
    {
        public static DependencyCollection AddEmitter(this DependencyCollection dependencies)
        {
            dependencies
                .AddFactory(new CommandPipelineFactory())
                .AddFactory(new NotificationPipelineFactory())
                .AddFactory(new QueryPipelineFactory())
                .AddScoped<IEmitter>(scope => new Emitter(scope));

            return dependencies;
        }

        public static DependencyCollection AddCommandBehaviour<TBehaviour>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var behaviourTypes = Types.CommandBehaviourTypes;
            var implementation = typeof(TBehaviour);

            if (!AddDependencies(dependencies, implementation, behaviourTypes, lifetime))
            {
                throw NotImplementedCommand(implementation, "behaviour");
            }

            return dependencies;
        }

        public static DependencyCollection AddCommandProcessor<TProcessor>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorTypes = Types.CommandProcessorTypes;

            if (!AddDependencies(dependencies, implementation, processorTypes, lifetime))
            {
                throw NotImplementedCommand(implementation, "processor");
            }

            return dependencies;
        }

        public static DependencyCollection AddCommandProcessor(this DependencyCollection dependencies, object processor)
        {
            var implementation = processor.GetType();
            var processorTypes = Types.CommandProcessorTypes;

            if (!AddInstance(dependencies, processor, processorTypes))
            {
                throw NotImplementedCommand(implementation, "processor");
            }

            return dependencies;
        }

        public static DependencyCollection AddNotificationProcessor<TProcessor>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorTypes = Types.NotificationProcessorTypes;

            if (!AddDependencies(dependencies, implementation, processorTypes, lifetime))
            {
                throw NotImplementedNotification(implementation);
            }

            return dependencies;
        }

        public static DependencyCollection AddNotificationProcessor(this DependencyCollection dependencies,
            object processor)
        {
            var implementation = processor.GetType();
            var processorTypes = Types.NotificationProcessorTypes;

            if (!AddInstance(dependencies, processor, processorTypes))
            {
                throw NotImplementedNotification(implementation);
            }

            return dependencies;
        }

        public static DependencyCollection AddQueryBehaviour<TBehaviour>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var behaviourTypes = Types.QueryBehaviourTypes;
            var implementation = typeof(TBehaviour);

            if (!AddDependencies(dependencies, implementation, behaviourTypes, lifetime))
            {
                throw NotImplementedQuery(implementation, "behaviour");
            }

            return dependencies;
        }

        public static DependencyCollection AddQueryProcessor<TProcessor>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            var implementation = typeof(TProcessor);
            var processorTypes = Types.QueryProcessorTypes;

            if (!AddDependencies(dependencies, implementation, processorTypes, lifetime))
            {
                throw NotImplementedQuery(implementation, "processor");
            }

            return dependencies;
        }

        public static DependencyCollection AddQueryProcessor(this DependencyCollection dependencies, object processor)
        {
            var implementation = processor.GetType();
            var processorTypes = Types.QueryProcessorTypes;

            if (!AddInstance(dependencies, processor, processorTypes))
            {
                throw NotImplementedQuery(implementation, "processor");
            }

            return dependencies;
        }

        public static DependencyScanner AddEmitterProcessors(this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            return scanner.UseAllover(new ProcessorsAllover(lifetime));
        }

        public static DependencyCollection CreateCommandProcessor<TCommand>(this DependencyCollection dependencies,
            Action<TCommand> processor)
            where TCommand : ICommand
        {
            return dependencies.AddInstance(new ActionCommandProcessor<TCommand>(processor));
        }

        public static DependencyCollection CreateCommandProcessor<TCommand, TContext>(
            this DependencyCollection dependencies, Action<TCommand, TContext> processor)
            where TCommand : ICommand
            where TContext : class
        {
            return dependencies.AddSingleton<ICommandProcessor<TCommand>>(scope =>
                new ActionCommandProcessor<TCommand, TContext>(processor,
                    scope.GetRequiredService<DependencyProvider>()));
        }

        public static DependencyCollection CreateQueryProcessor<TQuery, TResult>(this DependencyCollection dependencies,
            Func<TQuery, TResult> processor)
            where TQuery : IQuery<TResult>
        {
            return dependencies.AddInstance(new ActionQueryProcessor<TQuery, TResult>(processor));
        }

        public static DependencyCollection CreateQueryProcessor<TQuery, TContext, TResult>(
            this DependencyCollection dependencies, Func<TQuery, TContext, TResult> processor)
            where TQuery : IQuery<TResult>
            where TContext : class
        {
            return dependencies.AddSingleton<IQueryProcessor<TQuery, TResult>>(scope =>
                new ActionQueryProcessor<TQuery, TContext, TResult>(processor,
                    scope.GetRequiredService<DependencyProvider>()));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AddDependencies(
            DependencyCollection dependencies,
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
            dependencies.AddDependency(contracts.ToArray(), implementation, lifetime);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AddInstance(
            DependencyCollection dependencies,
            object instance,
            Type[] genericInterfaces)
        {
            var type = instance.GetType();
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(type, genericInterfaces);

            if (contracts.Length == 0)
            {
                return false;
            }

            dependencies.AddInstance(contracts.ToArray(), instance);
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