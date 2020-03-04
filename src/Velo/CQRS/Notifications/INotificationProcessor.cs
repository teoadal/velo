using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Notifications
{
    public interface INotificationProcessor<in TNotification>
        where TNotification: INotification
    {
        Task Process(TNotification notification, CancellationToken cancellationToken);
    }
}