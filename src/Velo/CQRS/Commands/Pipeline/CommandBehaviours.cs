using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Commands.Pipeline
{
    internal interface ICommandBehaviours<in TCommand> : IDisposable
        where TCommand : ICommand
    {
        bool HasBehaviours { get; }
        
        Task Execute(TCommand command, CancellationToken cancellationToken);
    }

    internal sealed class CommandBehaviours<TCommand> : ICommandBehaviours<TCommand>
        where TCommand : ICommand
    {
        public bool HasBehaviours => true;

        private ICommandBehaviour<TCommand>[] _behaviours;
        private CommandPipeline<TCommand> _pipeline;

        public CommandBehaviours(CommandPipeline<TCommand> pipeline, ICommandBehaviour<TCommand>[] behaviours)
        {
            _behaviours = behaviours;
            _pipeline = pipeline;
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            var closure = new Closure(this, command, cancellationToken);
            return closure.Execute();
        }

        private sealed class Closure
        {
            private readonly CommandBehaviours<TCommand> _context;
            private readonly TCommand _command;
            private readonly CancellationToken _cancellationToken;
            private readonly Func<Task> _next;

            private int _position;

            public Closure(CommandBehaviours<TCommand> context, TCommand command, CancellationToken cancellationToken)
            {
                _context = context;
                _command = command;
                _cancellationToken = cancellationToken;

                _next = Execute;

                _position = 0;
            }

            public Task Execute()
            {
                var behaviours = _context._behaviours;

                // ReSharper disable once InvertIf
                if ((uint) _position < (uint) behaviours.Length)
                {
                    var behaviour = behaviours[_position++];
                    return behaviour.Execute(_command, _next, _cancellationToken);
                }

                return _context._pipeline.RunProcessors(_command, _cancellationToken);
            }
        }

        public void Dispose()
        {
            _behaviours = null;
            _pipeline = null;
        }
    }

    internal sealed class NullCommandBehaviours<TCommand> : ICommandBehaviours<TCommand>
        where TCommand : ICommand
    {
        public static readonly ICommandBehaviours<TCommand> Instance = new NullCommandBehaviours<TCommand>();

        public bool HasBehaviours => false;
        
        private NullCommandBehaviours()
        {
        }

        public Task Execute(TCommand command, CancellationToken cancellationToken)
        {
            throw Error.InvalidOperation("No behaviours for execute");
        }

        public void Dispose()
        {
        }
    }
}