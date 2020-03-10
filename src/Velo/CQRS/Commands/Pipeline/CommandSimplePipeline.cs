using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed class CommandSimplePipeline<TCommand> : ICommandPipeline<TCommand>
        where TCommand : notnull, ICommand
    {
        private ICommandProcessor<TCommand> _processor;

        public CommandSimplePipeline(ICommandProcessor<TCommand> processor)
        {
            _processor = processor;
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            return _processor.Process(command, cancellationToken);
        }

        Task ICommandPipeline.Send(ICommand command, CancellationToken cancellationToken)
        {
            return Execute((TCommand) command, cancellationToken);
        }

        public void Dispose()
        {
            _processor = null;
        }
    }
}