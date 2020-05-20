using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed class CommandSimplePipeline<TCommand> : ICommandPipeline<TCommand>
        where TCommand : ICommand
    {
        private ICommandProcessor<TCommand> _processor;

        private bool _disposed;

        public CommandSimplePipeline(ICommandProcessor<TCommand> processor)
        {
            _processor = processor;
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(ICommandPipeline<TCommand>));

            return _processor.Process(command, cancellationToken);
        }

        Task ICommandPipeline.Send(ICommand command, CancellationToken cancellationToken)
        {
            return Execute((TCommand) command, cancellationToken);
        }

        public void Dispose()
        {
            _processor = null!;
            _disposed = true;
        }
    }
}