using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandPipeline<TCommand> : ICommandPipeline
        where TCommand : ICommand
    {
        private readonly CommandBehaviours<TCommand> _behaviours;
        private readonly ICommandPreProcessor<TCommand>[] _preProcessors;
        private readonly ICommandProcessor<TCommand> _processor;
        private readonly ICommandPostProcessor<TCommand>[] _postProcessors;

        public CommandPipeline(
            ICommandBehaviour<TCommand>[] behaviours,
            ICommandPreProcessor<TCommand>[] preProcessors,
            ICommandProcessor<TCommand> processor,
            ICommandPostProcessor<TCommand>[] postProcessors)
        {
            _behaviours = new CommandBehaviours<TCommand>(this, behaviours);
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public ValueTask Execute(TCommand command, CancellationToken cancellationToken)
        {
            return _behaviours.HasBehaviours
                ? _behaviours.Execute(command, cancellationToken)
                : RunProcessors(command, cancellationToken);
        }

        internal async ValueTask RunProcessors(TCommand command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var preProcessor in _preProcessors)
            {
                var preProcess = preProcessor.PreProcess(command, cancellationToken);
                if (!preProcess.IsCompletedSuccessfully)
                {
                    await preProcess;
                }
            }

            var process = _processor.Process(command, cancellationToken);
            if (!process.IsCompletedSuccessfully)
            {
                await process;
            }

            foreach (var postProcessor in _postProcessors)
            {
                var postProcess = postProcessor.PostProcess(command, cancellationToken);
                if (!postProcess.IsCompletedSuccessfully)
                {
                    await postProcess;
                }
            }
        }

        ValueTask ICommandPipeline.Send(ICommand command, CancellationToken cancellationToken)
        {
            return Execute((TCommand) command, cancellationToken);
        }
    }

    internal interface ICommandPipeline
    {
        ValueTask Send(ICommand command, CancellationToken cancellationToken);
    }
}