using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Notifications;

namespace Velo.TestsModels.Emitting.Parallel
{
    public class ParallelNotificationProcessor : INotificationProcessor<ParallelNotification>
    {
        public Task Process(ParallelNotification notification, CancellationToken cancellationToken)
        {
            return Task.Run(() => notification.ExecutedOn.Add(Task.CurrentId.Value), cancellationToken);
        }
    }
}