using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class PostProcessor : ICommandPostProcessor<Command>
    {
        private readonly IEmitter _emitter;

        public PostProcessor(IEmitter emitter)
        {
            _emitter = emitter;
        }

        public Task PostProcess(Command command, CancellationToken cancellationToken)
        {
            command.PostProcessed = true;
            
            var notification = new Notification {Id = command.Id};
            return _emitter.Publish(notification, cancellationToken);
        }
    }
}