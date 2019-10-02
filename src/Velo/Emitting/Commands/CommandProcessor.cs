using System;
using Velo.Ordering;

namespace Velo.Emitting.Commands
{
    internal sealed class CommandProcessor<TCommand> : ICommandProcessor<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand>[] _handlers;

        public CommandProcessor(ICommandHandler<TCommand>[] handlers)
        {
            var comparer = new OrderAttributeComparer<ICommandHandler<TCommand>>();
            Array.Sort(handlers, comparer);

            _handlers = handlers;
        }

        public void Execute(TCommand command)
        {
            var handlers = _handlers;
            var context = new HandlerContext<TCommand>(command);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                handlers[i].Execute(context);

                if (context.StopPropagation)
                {
                    break;
                }
            }
        }
    }
}