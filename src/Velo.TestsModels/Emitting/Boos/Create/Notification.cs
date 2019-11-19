using Velo.CQRS.Notifications;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class Notification : INotification
    {
        public int Id { get; set; }
        
        public bool StopPropagation { get; set; }
    }
}