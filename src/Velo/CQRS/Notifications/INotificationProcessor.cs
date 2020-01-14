using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications
{
    public interface INotificationProcessor<in TNotification> : INotificationProcessor
    {
        ValueTask Process(TNotification notification, CancellationToken cancellationToken);
    }

    public interface INotificationProcessor
    {
    }
}