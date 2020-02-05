using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications
{
    internal sealed class NotificationPipeline<TNotification> : INotificationPipeline
        where TNotification : INotification
    {
        private readonly INotificationProcessor<TNotification>[] _processors;

        public NotificationPipeline(INotificationProcessor<TNotification>[] processors)
        {
            _processors = processors;
        }

        public async ValueTask Publish(TNotification notification, CancellationToken cancellationToken)
        {
            foreach (var processor in _processors)
            {
                if (notification.StopPropagation || cancellationToken.IsCancellationRequested) break;
                await processor.Process(notification, cancellationToken);
            }
        }

        ValueTask INotificationPipeline.Publish(INotification notification, CancellationToken cancellationToken)
        {
            return Publish((TNotification) notification, cancellationToken);
        }
    }

    internal interface INotificationPipeline
    {
        ValueTask Publish(INotification notification, CancellationToken cancellationToken);
    }
}