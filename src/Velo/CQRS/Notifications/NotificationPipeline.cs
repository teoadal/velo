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
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var processor in _processors)
            {
                if (notification.StopPropagation) break;

                var process = processor.Process(notification, cancellationToken);
                if (!process.IsCompletedSuccessfully)
                {
                    await process;
                }
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