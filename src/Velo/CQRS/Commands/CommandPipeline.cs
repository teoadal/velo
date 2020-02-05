using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandPipeline<TCommand> : ICommandPipeline
        where TCommand : ICommand
    {
        private readonly ICommandPreProcessor<TCommand>[] _preProcessors;
        private readonly ICommandProcessor<TCommand> _processor;
        private readonly ICommandPostProcessor<TCommand>[] _postProcessors;

        public CommandPipeline(
            ICommandPreProcessor<TCommand>[] preProcessors,
            ICommandProcessor<TCommand> processor,
            ICommandPostProcessor<TCommand>[] postProcessors)
        {
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public async ValueTask Execute(TCommand command, CancellationToken cancellationToken)
        {
            foreach (var preProcessor in _preProcessors)
            {
                if (cancellationToken.IsCancellationRequested) return;
                await preProcessor.PreProcess(command, cancellationToken);
            }

            await _processor.Process(command, cancellationToken);

            foreach (var postProcessor in _postProcessors)
            {
                if (cancellationToken.IsCancellationRequested) return;
                await postProcessor.PostProcess(command, cancellationToken);
            }
        }

        ValueTask ICommandPipeline.Execute(ICommand command, CancellationToken cancellationToken)
        {
            return Execute((TCommand) command, cancellationToken);
        }
    }

    internal interface ICommandPipeline
    {
        ValueTask Execute(ICommand command, CancellationToken cancellationToken);
    }
}