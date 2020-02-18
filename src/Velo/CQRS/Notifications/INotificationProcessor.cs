using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications
{
    public interface INotificationProcessor<in TNotification>
    {
        Task Process(TNotification notification, CancellationToken cancellationToken);
    }
}