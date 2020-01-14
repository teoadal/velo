using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class PostProcessor : ICommandPostProcessor<Command>
    {
        private readonly Emitter _emitter;

        public PostProcessor(Emitter emitter)
        {
            _emitter = emitter;
        }

        public ValueTask PostProcess(Command command, CancellationToken cancellationToken)
        {
            command.PostProcessed = true;
            
            var notification = new Notification {Id = command.Id};
            return _emitter.Publish(notification, cancellationToken);
        }
    }
}