using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed class CommandPipeline<TCommand> : ICommandPipeline<TCommand>
        where TCommand : ICommand
    {
        private ICommandBehaviours<TCommand> _behaviours;
        private ICommandPreProcessor<TCommand>[] _preProcessors;
        private ICommandProcessor<TCommand> _processor;
        private ICommandPostProcessor<TCommand>[] _postProcessors;

        public CommandPipeline(
            ICommandBehaviour<TCommand>[] behaviours,
            ICommandPreProcessor<TCommand>[] preProcessors,
            ICommandProcessor<TCommand> processor,
            ICommandPostProcessor<TCommand>[] postProcessors)
        {
            _behaviours = behaviours.Length > 0
                ? new CommandBehaviours<TCommand>(this, behaviours)
                : NullCommandBehaviours<TCommand>.Instance;

            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public CommandPipeline(ICommandProcessor<TCommand> processor)
        {
            _behaviours = NullCommandBehaviours<TCommand>.Instance;
            _preProcessors = Array.Empty<ICommandPreProcessor<TCommand>>();
            _processor = processor;
            _postProcessors = Array.Empty<ICommandPostProcessor<TCommand>>();
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            return _behaviours.HasBehaviours
                ? _behaviours.Execute(command, cancellationToken)
                : RunProcessors(command, cancellationToken);
        }

        internal async Task RunProcessors(TCommand command, CancellationToken cancellationToken)
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