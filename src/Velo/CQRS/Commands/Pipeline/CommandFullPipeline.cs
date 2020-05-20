using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed partial class CommandFullPipeline<TCommand> : ICommandPipeline<TCommand>
        where TCommand : ICommand
    {
        private BehaviourContext _behaviours;
        private ICommandPreProcessor<TCommand>[] _preProcessors;
        private ICommandProcessor<TCommand> _processor;
        private ICommandPostProcessor<TCommand>[] _postProcessors;

        private bool _disposed;

        public CommandFullPipeline(
            ICommandBehaviour<TCommand>[] behaviours,
            ICommandPreProcessor<TCommand>[] preProcessors,
            ICommandProcessor<TCommand> processor,
            ICommandPostProcessor<TCommand>[] postProcessors)
        {
            _behaviours = new BehaviourContext(this, behaviours);
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;

            _disposed = false;
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(ICommandPipeline<TCommand>));

            return _behaviours.Execute(command, cancellationToken);
        }

        private async Task RunProcessors(TCommand command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var preProcessor in _preProcessors)
            {
                await preProcessor.PreProcess(command, cancellationToken);
            }

            await _processor.Process(command, cancellationToken);

            foreach (var postProcessor in _postProcessors)
            {
                await postProcessor.PostProcess(command, cancellationToken);
            }
        }

        Task ICommandPipeline.Send(ICommand command, CancellationToken cancellationToken)
        {
            return Execute((TCommand) command, cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _behaviours.Dispose();

            _behaviours = null!;
            _preProcessors = null!;
            _processor = null!;
            _postProcessors = null!;

            _disposed = true;
        }
    }
}