using System.Collections.Concurrent;
using Velo.CQRS.Notifications;
using Velo.Ordering;

namespace Velo.TestsModels.Emitting.Parallel
{
    [Parallel]
    public class ParallelNotification : INotification
    {
        public ConcurrentBag<int?> ExecutedOn;

        public bool StopPropagation { get; set; }
        
        public ParallelNotification()
        {
            ExecutedOn = new ConcurrentBag<int?>();
        }

    }
}