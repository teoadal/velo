using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications
{
    internal sealed class NotificationPublisher<TNotification> : INotificationPublisher
        where TNotification : INotification
    {
        private readonly Type _processorsType;

        public NotificationPublisher()
        {
            _processorsType = typeof(INotificationProcessor<TNotification>[]);
        }

        public async Task Publish(IServiceProvider scope, TNotification notification, CancellationToken cancellationToken)
        {
            var serviceArray = scope.GetService(_processorsType);
            var handlers = (INotificationProcessor<TNotification>[]) serviceArray;

            foreach (var handler in handlers)
            {
                if (notification.StopPropagation || cancellationToken.IsCancellationRequested) break;

                await handler.Process(notification, cancellationToken);
            }
        }
    }

    internal interface INotificationPublisher
    {
    }
}