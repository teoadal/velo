using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS;
using Velo.CQRS.Notifications;
using Velo.TestsModels.Emitting.Boos.Create;

namespace Velo.TestsModels.Emitting.Foos.Create
{
    public class OnBooCreated : INotificationProcessor<Notification>
    {
        private readonly IEmitter _emitter;

        public OnBooCreated(IEmitter emitter)
        {
            _emitter = emitter;
        }

        public Task Process(Notification notification, CancellationToken cancellationToken)
        {
            var command = new Command {Id = notification.Id, Type = ModelType.Boo};
            return _emitter.Execute(command, cancellationToken);
        }
    }
}