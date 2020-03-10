using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed partial class CommandFullPipeline<TCommand>
    {
        private sealed class BehaviourContext
        {
            private ICommandBehaviour<TCommand>[] _behaviours;
            private CommandFullPipeline<TCommand> _pipeline;

            public BehaviourContext(CommandFullPipeline<TCommand> pipeline, ICommandBehaviour<TCommand>[] behaviours)
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
                private readonly BehaviourContext _context;
                private readonly TCommand _command;
                private readonly CancellationToken _cancellationToken;
                private readonly Func<Task> _next;

                private int _position;

                public Closure(BehaviourContext context, TCommand command, CancellationToken cancellationToken)
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
        
    }
    
}