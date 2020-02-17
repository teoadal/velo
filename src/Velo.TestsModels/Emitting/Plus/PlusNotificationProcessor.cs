using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Notifications;

namespace Velo.TestsModels.Emitting.Plus
{
    public sealed class PlusNotificationProcessor : INotificationProcessor<PlusNotification>
    {
        public ValueTask Process(PlusNotification notification, CancellationToken cancellationToken)
        {
            notification.Counter++;
            return new ValueTask();
        }
    }
}