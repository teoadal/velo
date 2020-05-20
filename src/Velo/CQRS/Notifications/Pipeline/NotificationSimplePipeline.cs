using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;
using Velo.Utils;

namespace Velo.CQRS.Notifications.Pipeline
{
    internal sealed class NotificationSimplePipeline<TNotification> : INotificationPipeline<TNotification>
        where TNotification : INotification
    {
        private INotificationProcessor<TNotification> _processor;

        private bool _disposed;
        
        public NotificationSimplePipeline(INotificationProcessor<TNotification> processor)
        {
            _processor = processor;
        }

        public Task Publish(TNotification notification, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(INotificationPipeline<TNotification>));
            
            return notification.StopPropagation
                ? TaskUtils.CompletedTask
                : _processor.Process(notification, cancellationToken);
        }

        Task INotificationPipeline.Publish(INotification notification, CancellationToken cancellationToken)
        {
            return Publish((TNotification) notification, cancellationToken);
        }

        public void Dispose()
        {
            _processor = null!;
            _disposed = true;
        }
    }
}