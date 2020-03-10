using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed partial class CommandFullPipeline<TCommand> : ICommandPipeline<TCommand>
        where TCommand : notnull, ICommand
    {
        private BehaviourContext _behaviours;
        private ICommandPreProcessor<TCommand>[] _preProcessors;
        private ICommandProcessor<TCommand> _processor;
        private ICommandPostProcessor<TCommand>[] _postProcessors;

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
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
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
            _behaviours.Dispose();

            _behaviours = null;
            _preProcessors = null;
            _processor = null;
            _postProcessors = null;
        }
    }
}