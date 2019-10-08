using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.Emitting.Commands.Processors
{
    internal class SyncCommandProcessor<TCommand> : CommandProcessor<TCommand>, IAsyncCommandProcessor<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand>[] _handlers;

        public SyncCommandProcessor(IReadOnlyList<ICommandHandler> handlers)
        {
            _handlers = new ICommandHandler<TCommand>[handlers.Count];
            for (var i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = (ICommandHandler<TCommand>) handlers[i];
            }
        }

        public override void Execute(TCommand command)
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