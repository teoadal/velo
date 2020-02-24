using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Ordering;

namespace Velo.CQRS.Notifications
{
    internal sealed class NotificationPipeline<TNotification> : INotificationPipeline
        where TNotification : INotification
    {
        private static readonly bool HasParallelAttribute = Attribute.IsDefined(typeof(TNotification), typeof(ParallelAttribute));
        private INotificationProcessor<TNotification>[] _processors;

        public NotificationPipeline(INotificationProcessor<TNotification>[] processors)
        {
            _processors = processors;
        }

        public Task Publish(TNotification notification, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return HasParallelAttribute
                ? PublishParallel(notification, cancellationToken)
                : PublishSequential(notification, cancellationToken);
        }

        private Task PublishParallel(TNotification notification, CancellationToken cancellationToken)
        {
            var tasks = new Task[_processors.Length];
            for (var i = 0; i < _processors.Length; i++)
            {
                tasks[i] = _processors[i].Process(notification, cancellationToken);
            }

            return Task.WhenAll(tasks);
        }

        private async Task PublishSequential(TNotification notification, CancellationToken cancellationToken)
        {
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

    internal interface INotificationPipeline : IDisposable
    {
        Task Publish(INotification notification, CancellationToken cancellationToken);
    }
}