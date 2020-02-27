using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<Type, Type> ResolvedTypes = new Dictionary<Type, Type>();

        public static Type GetForCommand(Type commandType)
        {
            if (ResolvedTypes.TryGetValue(commandType, out var exists)) return exists;

            var pipelineType = Command.MakeGenericType(commandType);
            using (Lock.Enter(ResolvedTypes))
            {
                ResolvedTypes[commandType] = pipelineType;
            }

            return pipelineType;
        }

        public static Type GetForQuery(Type queryType)
        {
            if (ResolvedTypes.TryGetValue(queryType, out var exists)) return exists;

            var resultType = ReflectionUtils.GetGenericInterfaceParameters(queryType, QueryType)[0];
            var pipelineType = Query.MakeGenericType(queryType, resultType);

            using (Lock.Enter(ResolvedTypes))
            {
                ResolvedTypes[queryType] = pipelineType;
            }

            return pipelineType;
        }

        public static Type GetForNotification(Type notificationType)
        {
            if (ResolvedTypes.TryGetValue(notificationType, out var exists)) return exists;

            var pipelineType = Notification.MakeGenericType(notificationType);
            using (Lock.Enter(ResolvedTypes))
            {
                ResolvedTypes[notificationType] = pipelineType;
            }

            return pipelineType;
        }
    }
}