using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications.Pipeline
{
    internal sealed class NotificationSequentialPipeline<TNotification> : INotificationPipeline<TNotification>
        where TNotification : INotification
    {
        private INotificationProcessor<TNotification>[] _processors;

        public NotificationSequentialPipeline(INotificationProcessor<TNotification>[] processors)
        {
            _processors = processors;
        }

        public async Task Publish(TNotification notification, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var processor in _processors)
            {
                if (notification.StopPropagation) break;

                await processor.Process(notification, cancellationToken);
            }
        }

        Task INotificationPipeline.Publish(INotification notification, CancellationToken cancellationToken)
        {
            return Publish((TNotification) notification, cancellationToken);
        }

        public void Dispose()
        {
            _processors = null;
        }
    }
}