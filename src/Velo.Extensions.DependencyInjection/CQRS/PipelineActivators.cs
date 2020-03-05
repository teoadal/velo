using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.Threading;
using Velo.Utils;

namespace Velo.Extensions.DependencyInjection.CQRS
{
    internal static class PipelineActivators
    {
        public static Func<IServiceProvider, object> GetCommandPipelineActivator(Type commandType)
        {
            return GetMethod(nameof(CommandActivator), commandType);
        }

        public static Func<IServiceProvider, object> GetNotificationPipelineActivator(Type notificationType)
        {
            return GetMethod(nameof(NotificationsActivator), notificationType);
        }

        public static Func<IServiceProvider, object> GetQueryPipelineActivator(Type queryType, Type resultType)
        {
            return GetMethod(nameof(QueryActivator), queryType, resultType);
        }

        private static object CommandActivator<TCommand>(IServiceProvider provider)
            where TCommand : ICommand
        {
            var behaviours = provider.GetServices<ICommandBehaviour<TCommand>>().ToArray();
            var preProcessors = provider.GetServices<ICommandPreProcessor<TCommand>>().ToArray();
            var processor = provider.GetRequiredService<ICommandProcessor<TCommand>>();
            var postProcessors = provider.GetServices<ICommandPostProcessor<TCommand>>().ToArray();

            return new CommandPipeline<TCommand>(behaviours, preProcessors, processor, postProcessors);
        }

        private static object NotificationsActivator<TNotification>(IServiceProvider provider)
            where TNotification : INotification
        {
            var processors = provider
                .GetServices<INotificationProcessor<TNotification>>()
                .ToArray();

            return processors.Length switch
            {
                0 => throw Error.DependencyNotRegistered(typeof(INotificationProcessor<TNotification>)),
                1 => new NotificationSimplePipeline<TNotification>(processors[0]),
                _ => (ParallelAttribute.IsDefined(typeof(TNotification))
                    ? (INotificationPipeline<TNotification>) new NotificationParallelPipeline<TNotification>(processors)
                    : new NotificationSequentialPipeline<TNotification>(processors))
            };
        }

        private static object QueryActivator<TQuery, TResult>(IServiceProvider provider)
            where TQuery : IQuery<TResult>
        {
            var behaviours = provider.GetServices<IQueryBehaviour<TQuery, TResult>>().ToArray();
            var preProcessors = provider.GetServices<IQueryPreProcessor<TQuery, TResult>>().ToArray();
            var processor = provider.GetRequiredService<IQueryProcessor<TQuery, TResult>>();
            var postProcessors = provider.GetServices<IQueryPostProcessor<TQuery, TResult>>().ToArray();

            if (behaviours.Length > 0)
            {
                return new QueryFullPipeline<TQuery, TResult>(behaviours, preProcessors, processor, postProcessors);
            }
            
            if (preProcessors.Length > 0 || postProcessors.Length > 0)
            {
                return new QuerySequentialPipeline<TQuery, TResult>(preProcessors, processor, postProcessors);
            }
            
            return new QuerySimplePipeline<TQuery, TResult>(processor);
        }

        private static Func<IServiceProvider, object> GetMethod(string name, params Type[] genericParameters)
        {
            // ReSharper disable once PossibleNullReferenceException
            var method = typeof(PipelineActivators)
                .GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(genericParameters);

            var methodDelegate = method.CreateDelegate(typeof(Func<IServiceProvider, object>), null);
            return (Func<IServiceProvider, object>) methodDelegate;
        }
    }
}