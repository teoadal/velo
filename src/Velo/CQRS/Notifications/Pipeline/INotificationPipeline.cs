using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications.Pipeline
{
    internal interface INotificationPipeline<in TNotification> : INotificationPipeline
        where TNotification : INotification
    {
        Task Publish(TNotification notification, CancellationToken cancellationToken);
    }

    internal interface INotificationPipeline : IDisposable
    {
        Task Publish(INotification notification, CancellationToken cancellationToken);
    }
}