using System;
using Velo.Ordering;

namespace Velo.CQRS.Commands
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

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                handlers[i].Execute(command);

                if (command.StopPropagation)
                {
                    break;
                }
            }
        }
    }
}