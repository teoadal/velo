using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandPipeline<TCommand> : ICommandPipeline
        where TCommand : ICommand
    {
        private readonly ICommandBehaviour<TCommand>[] _behaviours;
        private readonly ICommandPreProcessor<TCommand>[] _preProcessors;
        private readonly ICommandProcessor<TCommand> _processor;
        private readonly ICommandPostProcessor<TCommand>[] _postProcessors;

        public CommandPipeline(
            ICommandBehaviour<TCommand>[] behaviours,
            ICommandPreProcessor<TCommand>[] preProcessors,
            ICommandProcessor<TCommand> processor,
            ICommandPostProcessor<TCommand>[] postProcessors)
        {
            _behaviours = behaviours;
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public ValueTask Execute(TCommand command, CancellationToken cancellationToken)
        {
            // ValueTask Handler() => RunProcessors(command, cancellationToken);
            //
            // Func<ValueTask> NextStep(Func<ValueTask> next, ICommandBehaviour<TCommand> pipeline) =>
            //     () => pipeline.Execute(command, next, cancellationToken);
            //
            // var aggregation = _behaviours.Aggregate((Func<ValueTask>) Handler, NextStep);
            // return aggregation();

            if (_behaviours.Length == 0) return RunProcessors(command, cancellationToken);

            var closure = new CommandBehaviours<TCommand>(command, _behaviours, this, cancellationToken);
            return closure.Execute();
        }

        internal async ValueTask RunProcessors(TCommand command, CancellationToken cancellationToken)
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