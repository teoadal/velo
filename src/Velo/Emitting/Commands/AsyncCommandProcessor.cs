using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.Emitting.Commands
{
    internal interface IAsyncCommandProcessor<in TCommand> : ICommandProcessor
        where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }
    
    internal sealed class AsyncCommandProcessor<TCommand>: IAsyncCommandProcessor<TCommand>, ICommandProcessor<TCommand>
        where TCommand: ICommand
    {
        private readonly Type _commandType;
        private readonly IAsyncCommandHandler<TCommand>[] _handlers;

        public AsyncCommandProcessor(IReadOnlyList<ICommandHandler> handlers)
        {
            _commandType = Typeof<TCommand>.Raw;
            _handlers = new IAsyncCommandHandler<TCommand>[handlers.Count];
            for (var i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = (IAsyncCommandHandler<TCommand>) handlers[i];
            }
        }

        public bool Applicable(Type type)
        {
            return _commandType.IsAssignableFrom(type);
        }
        
        public void Execute(TCommand command)
        {
            ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();
        }
        
        public async Task ExecuteAsync(TCommand command, CancellationToken cancellationToken)
        {
            var context = new HandlerContext<TCommand>(command);
            var handlers = _handlers;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                await handler.ExecuteAsync(context, cancellationToken);

                if (context.StopPropagation)
                {
                    break;
                }
            }
        }
    }
}