using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal sealed partial class CommandFullPipeline<TCommand>
    {
        private sealed class BehaviourContext
        {
            [ThreadStatic] 
            private static Closure? _closure;

            private ICommandBehaviour<TCommand>[] _behaviours;
            private CommandFullPipeline<TCommand> _pipeline;

            public BehaviourContext(CommandFullPipeline<TCommand> pipeline, ICommandBehaviour<TCommand>[] behaviours)
            {
                _closure = null;
                _behaviours = behaviours;
                _pipeline = pipeline;
            }

            public Task Execute(TCommand command, CancellationToken cancellationToken)
            {
                Closure closure = _closure ?? new Closure(this);
                
                _closure = null;

                closure.CancellationToken = cancellationToken;
                closure.Command = command;

                return closure.Execute();
            }

            private sealed class Closure : IDisposable
            {
                public CancellationToken CancellationToken;
                public TCommand Command;

                private BehaviourContext _context;
                private Func<Task> _next;

                private int _position;

                public Closure(BehaviourContext context)
                {
                    _context = context;
                    _next = Execute;

                    Command = default!;
                    CancellationToken = default;

                    _position = 0;
                }

                public Task Execute()
                {
                    var behaviours = _context._behaviours;

                    // ReSharper disable once InvertIf
                    if ((uint) _position < (uint) behaviours.Length)
                    {
                        var behaviour = behaviours[_position++];
                        return behaviour.Execute(Command, _next, CancellationToken);
                    }

                    var token = CancellationToken;
                    var command = Command;
                    var pipeline = _context._pipeline;
                    
                    Clear();
                    
                    return pipeline.RunProcessors(command, token);
                }

                private void Clear()
                {
                    CancellationToken = default;
                    Command = default!;

                    _position = 0;
                    
                    _closure = this;
                }

                public void Dispose()
                {
                    _context = null!;
                    _next = null!;
                }
            }

            public void Dispose()
            {
                _behaviours = null!;

                _closure?.Dispose();
                _closure = null;

                _pipeline = null!;
            }
        }
    }
}