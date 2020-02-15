using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications
{
    public interface INotificationProcessor<in TNotification>
    {
        ValueTask Process(TNotification notification, CancellationToken cancellationToken);
    }
}