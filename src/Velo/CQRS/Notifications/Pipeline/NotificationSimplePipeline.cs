using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications.Pipeline
{
    internal sealed class NotificationSimplePipeline<TNotification>: INotificationPipeline<TNotification>
        where TNotification: INotification
    {
        private INotificationProcessor<TNotification> _processor;

        public NotificationSimplePipeline(INotificationProcessor<TNotification> processor)
        {
            _processor = processor;
        }

        public Task Publish(TNotification notification, CancellationToken cancellationToken)
        {
            return _processor.Process(notification, cancellationToken);
        }

        Task INotificationPipeline.Publish(INotification notification, CancellationToken cancellationToken)
        {
            return Publish((TNotification) notification, cancellationToken);
        }
        
        public void Dispose()
        {
            _processor = null;
        }
    }
}