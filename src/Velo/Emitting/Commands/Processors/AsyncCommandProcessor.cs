using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Commands.Processors
{
    internal interface IAsyncCommandProcessor<in TCommand> : ICommandProcessor
        where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }

    internal sealed class AsyncCommandProcessor<TCommand> : CommandProcessor<TCommand>, IAsyncCommandProcessor<TCommand>
        where TCommand : ICommand
    {
        private readonly IAsyncCommandHandler<TCommand>[] _handlers;

        public AsyncCommandProcessor(IReadOnlyList<ICommandHandler> handlers)
        {
            _handlers = new IAsyncCommandHandler<TCommand>[handlers.Count];
            for (var i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = (IAsyncCommandHandler<TCommand>) handlers[i];
            }
        }

        public override void Execute(TCommand command)
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