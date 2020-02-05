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
        private static readonly Type CommandPipelineType = typeof(CommandPipeline<>);

        private static readonly Type QueryPipelineType = typeof(QueryPipeline<,>);
        private static readonly Type QueryType = typeof(IQuery<>);

        private static readonly Type NotificationPipelineType = typeof(NotificationPipeline<>);

        private static readonly ConcurrentDictionary<Type, Type> ResolvedTypes = new ConcurrentDictionary<Type, Type>();

        private static readonly Func<Type, Type> CommandBuilder = t => CommandPipelineType.MakeGenericType(t);
        private static readonly Func<Type, Type> QueryBuilder = BuildQueryPipelineType;
        private static readonly Func<Type, Type> NotificationBuilder = t => NotificationPipelineType.MakeGenericType(t);

        public static Type GetForCommand(Type commandType)
        {
            return ResolvedTypes.GetOrAdd(commandType, CommandBuilder);
        }

        public static Type GetForQuery(Type queryType)
        {
            return ResolvedTypes.GetOrAdd(queryType, QueryBuilder);
        }

        public static Type GetForNotification(Type notificationType)
        {
            return ResolvedTypes.GetOrAdd(notificationType, NotificationBuilder);
        }

        private static Type BuildQueryPipelineType(Type queryType)
        {
            var resultType = ReflectionUtils.GetGenericInterfaceParameters(queryType, QueryType)[0];
            return QueryPipelineType.MakeGenericType(queryType, resultType);
        }
    }
}