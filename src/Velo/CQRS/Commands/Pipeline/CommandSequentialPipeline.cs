using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed class CommandSequentialPipeline<TCommand> : ICommandPipeline<TCommand>
        where TCommand : notnull, ICommand
    {
        private ICommandPreProcessor<TCommand>[] _preProcessors;
        private ICommandProcessor<TCommand> _processor;
        private ICommandPostProcessor<TCommand>[] _postProcessors;

        public CommandSequentialPipeline(
            ICommandPreProcessor<TCommand>[] preProcessors,
            ICommandProcessor<TCommand> processor,
            ICommandPostProcessor<TCommand>[] postProcessors)
        {
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public async Task Execute(TCommand command, CancellationToken cancellationToken)
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
            _preProcessors = null;
            _processor = null;
            _postProcessors = null;
        }
    }
}