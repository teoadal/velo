namespace Velo.CQRS.Notifications
{
    public interface INotification
    {
        bool StopPropagation { get; }
    }
}