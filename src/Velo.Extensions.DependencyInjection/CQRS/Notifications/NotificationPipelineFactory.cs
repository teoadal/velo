using System;
using System.Reflection;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.Threading;
using Velo.Utils;

namespace Velo.Extensions.DependencyInjection.CQRS.Notifications
{
    internal static class NotificationPipelineFactory
    {
        private static readonly MethodInfo ActivatorMethod;
        
        static NotificationPipelineFactory()
        {
            ActivatorMethod = typeof(NotificationPipelineFactory)
                .GetMethod(nameof(Activator), BindingFlags.Static | BindingFlags.NonPublic);
        }
        
        public static Func<IServiceProvider, object> GetActivator(Type notificationType)
        {
            var method = ActivatorMethod.MakeGenericMethod(notificationType);
            return ReflectionUtils.BuildStaticMethodDelegate<Func<IServiceProvider, object>>(method);
        }
        
        private static object Activator<TNotification>(IServiceProvider provider)
            where TNotification : INotification
        {
            var processors = provider.GetArray<INotificationProcessor<TNotification>>();

            return processors.Length switch
            {
                0 => throw Error.DependencyNotRegistered(typeof(INotificationProcessor<TNotification>)),
                1 => new NotificationSimplePipeline<TNotification>(processors[0]),
                _ => (ParallelAttribute.IsDefined(typeof(TNotification))
                    ? (INotificationPipeline<TNotification>) new NotificationParallelPipeline<TNotification>(processors)
                    : new NotificationSequentialPipeline<TNotification>(processors))
            };
        }
    }
}