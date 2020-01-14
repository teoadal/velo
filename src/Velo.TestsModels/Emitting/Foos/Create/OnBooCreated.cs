using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS;
using Velo.CQRS.Notifications;
using Velo.TestsModels.Emitting.Boos.Create;

namespace Velo.TestsModels.Emitting.Foos.Create
{
    public class OnBooCreated : INotificationProcessor<Notification>
    {
        private readonly Emitter _emitter;

        public OnBooCreated(Emitter emitter)
        {
            _emitter = emitter;
        }

        public ValueTask Process(Notification notification, CancellationToken cancellationToken)
        {
            var command = new Command {Id = notification.Id, Type = ModelType.Boo};
            return _emitter.Execute(command, cancellationToken);
        }
    }
}