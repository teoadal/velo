using System;
using System.Collections.Concurrent;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;
using Velo.Utils;

namespace Velo.CQRS.Pipeline
{
    internal static class PipelineTypes
    {
        public static readonly Type Command = typeof(CommandPipeline<>);

        public static readonly Type[] CommandProcessorTypes =
        {
            typeof(ICommandPreProcessor<>),
            typeof(ICommandProcessor<>),
            typeof(ICommandPostProcessor<>)
        };

        public static readonly Type[] CommandBehaviourTypes = {typeof(ICommandBehaviour<>)};

        public static readonly Type Notification = typeof(NotificationPipeline<>);
        public static readonly Type[] NotificationProcessorTypes = {typeof(INotificationProcessor<>)};

        public static readonly Type Query = typeof(QueryPipeline<,>);

        public static readonly Type[] QueryProcessorTypes =
        {
            typeof(IQueryPreProcessor<,>),
            typeof(IQueryProcessor<,>),
            typeof(IQueryPostProcessor<,>)
        };

        public static readonly Type[] QueryBehaviourTypes = {typeof(IQueryBehaviour<,>)};
        private static readonly Type QueryType = typeof(IQuery<>);

        private static readonly ConcurrentDictionary<Type, Type> ResolvedTypes = new ConcurrentDictionary<Type, Type>();

        // ReSharper disable ConvertClosureToMethodGroup
        private static readonly Func<Type, Type> CommandPipelineTypeBuilder = t => Command.MakeGenericType(t);
        private static readonly Func<Type, Type> QueryPipelineTypeBuilder = t => BuildQueryPipelineType(t);
        private static readonly Func<Type, Type> NotificationPipelineTypeBuilder = t => Notification.MakeGenericType(t);
        // ReSharper restore ConvertClosureToMethodGroup

        public static Type GetForCommand(Type commandType)
        {
            return ResolvedTypes.GetOrAdd(commandType, CommandPipelineTypeBuilder);
        }

        public static Type GetForQuery(Type queryType)
        {
            return ResolvedTypes.GetOrAdd(queryType, QueryPipelineTypeBuilder);
        }

        public static Type GetForNotification(Type notificationType)
        {
            return ResolvedTypes.GetOrAdd(notificationType, NotificationPipelineTypeBuilder);
        }

        private static Type BuildQueryPipelineType(Type queryType)
        {
            var resultType = ReflectionUtils.GetGenericInterfaceParameters(queryType, QueryType)[0];
            return Query.MakeGenericType(queryType, resultType);
        }
    }
}