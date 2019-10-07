using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.Emitting.Commands
{
    internal class SyncCommandProcessor<TCommand> : ICommandProcessor<TCommand>, IAsyncCommandProcessor<TCommand>
        where TCommand : ICommand
    {
        private readonly Type _commandType;
        private readonly ICommandHandler<TCommand>[] _handlers;

        public SyncCommandProcessor(IReadOnlyList<ICommandHandler> handlers)
        {
            _commandType = Typeof<TCommand>.Raw;
            _handlers = new ICommandHandler<TCommand>[handlers.Count];
            for (var i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = (ICommandHandler<TCommand>) handlers[i];
            }
        }

        public bool Applicable(Type type)
        {
            return _commandType.IsAssignableFrom(type);
        }
        
        public void Execute(TCommand command)
        {
            var context = new HandlerContext<TCommand>(command);
            var handlers = _handlers;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                handler.Execute(context);

                if (context.StopPropagation)
                {
                    break;
                }
            }
        }

        public Task ExecuteAsync(TCommand command, CancellationToken cancellationToken)
        {
            Execute(command);
            return Task.CompletedTask;
        }
    }
}