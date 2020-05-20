using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Notifications.Pipeline
{
    internal sealed class NotificationParallelPipeline<TNotification> : INotificationPipeline<TNotification>
        where TNotification : INotification
    {
        private INotificationProcessor<TNotification>[] _processors;

        private bool _disposed;

        public NotificationParallelPipeline(INotificationProcessor<TNotification>[] processors)
        {
            _processors = processors;
        }

        public Task Publish(TNotification notification, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(INotificationPipeline<TNotification>));

            cancellationToken.ThrowIfCancellationRequested();

            var tasks = new Task[_processors.Length];
            for (var i = _processors.Length - 1; i >= 0; i--) // reverse for check bounds once
            {
                tasks[i] = _processors[i].Process(notification, cancellationToken);
            }

            return Task.WhenAll(tasks);
        }

        Task INotificationPipeline.Publish(INotification notification, CancellationToken cancellationToken)
        {
            return Publish((TNotification) notification, cancellationToken);
        }

        public void Dispose()
        {
            _processors = null!;
            _disposed = true;
        }
    }
}