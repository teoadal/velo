using Velo.CQRS.Notifications;

namespace Velo.TestsModels.Emitting.Plus
{
    public sealed class PlusNotification : INotification
    {
        public int Counter { get; set; }
        
        public bool StopPropagation { get; }
    }
}