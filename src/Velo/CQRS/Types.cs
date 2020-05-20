using System;
using System.Collections.Concurrent;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.Utils;

namespace Velo.CQRS
{
    // ReSharper disable once InconsistentNaming
    internal static class Types
    {
        public static readonly Type CommandPipeline = typeof(ICommandPipeline<>);

        public static readonly Type[] CommandProcessorTypes =
        {
            typeof(ICommandPreProcessor<>),
            typeof(ICommandProcessor<>),
            typeof(ICommandPostProcessor<>)
        };

        public static readonly Type[] CommandBehaviourTypes = {typeof(ICommandBehaviour<>)};

        public static readonly Type NotificationPipeline = typeof(INotificationPipeline<>);
        public static readonly Type[] NotificationProcessorTypes = {typeof(INotificationProcessor<>)};

        public static readonly Type QueryPipeline = typeof(IQueryPipeline<,>);

        public static readonly Type[] QueryProcessorTypes =
        {
            typeof(IQueryPreProcessor<,>),
            typeof(IQueryProcessor<,>),
            typeof(IQueryPostProcessor<,>)
        };

        public static readonly Type[] QueryBehaviourTypes = {typeof(IQueryBehaviour<,>)};
        private static readonly Type Query = typeof(IQuery<>);

        private static readonly ConcurrentDictionary<Type, Type> ResolvedTypes = new ConcurrentDictionary<Type, Type>();

        // ReSharper disable ConvertClosureToMethodGroup
        private static readonly Func<Type, Type> CommandPipelineTypeBuilder = t => CommandPipeline.MakeGenericType(t);
        private static readonly Func<Type, Type> QueryPipelineTypeBuilder = BuildQueryPipelineType;
        private static readonly Func<Type, Type> NotificationPipelineTypeBuilder = t => NotificationPipeline.MakeGenericType(t);
        // ReSharper restore ConvertClosureToMethodGroup

        public static Type GetCommandPipelineType(Type commandType)
        {
            return ResolvedTypes.GetOrAdd(commandType, CommandPipelineTypeBuilder);
        }

        public static Type GetQueryPipelineType(Type queryType)
        {
            return ResolvedTypes.GetOrAdd(queryType, QueryPipelineTypeBuilder);
        }

        public static Type GetNotificationPipelineType(Type notificationType)
        {
            return ResolvedTypes.GetOrAdd(notificationType, NotificationPipelineTypeBuilder);
        }

        private static Type BuildQueryPipelineType(Type queryType)
        {
            var resultType = ReflectionUtils.GetGenericInterfaceParameters(queryType, Query)[0];
            return QueryPipeline.MakeGenericType(queryType, resultType);
        }
    }
}