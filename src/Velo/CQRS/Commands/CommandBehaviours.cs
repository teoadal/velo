using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandBehaviours<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandBehaviour<TCommand>[] _behaviours;
        private readonly TCommand _command;
        private readonly CancellationToken _cancellationToken;
        private readonly Func<ValueTask> _next;
        private readonly CommandPipeline<TCommand> _pipeline;

        private int _position;

        public CommandBehaviours(
            TCommand command,
            ICommandBehaviour<TCommand>[] behaviours,
            CommandPipeline<TCommand> pipeline,
            CancellationToken cancellationToken)
        {
            _behaviours = behaviours;
            _command = command;
            _pipeline = pipeline;
            _cancellationToken = cancellationToken;

            // ReSharper disable once ConvertClosureToMethodGroup
            _next = () => Execute();

            _position = 0;
        }

        public ValueTask Execute()
        {
            // ReSharper disable once InvertIf
            if ((uint) _position < (uint) _behaviours.Length)
            {
                var behaviour = _behaviours[_position++];
                return behaviour.Execute(_command, _next, _cancellationToken);
            }

            return _pipeline.RunProcessors(_command, _cancellationToken);
        }
    }
}