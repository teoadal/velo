using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandHandlerCollection
    {
        private readonly Dictionary<int, ICommandHandler> _handlers;

        public CommandHandlerCollection(DependencyContainer container)
        {
            var handlers = container.Resolve<ICommandHandler[]>();
            var commandHandlerGenericType = typeof(ICommandHandler<>);

            _handlers = new Dictionary<int, ICommandHandler>(handlers.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var commandType = ReflectionUtils.GetGenericInterfaceParameters(handlerType, commandHandlerGenericType);
                var commandTypeId = Typeof.GetTypeId(commandType[0]);

                _handlers.Add(commandTypeId, handler);
            }
        }

        public ICommandHandler<TCommand> GetProcessor<TCommand>() where TCommand : ICommand
        {
            var commandId = Typeof<TCommand>.Id;

            if (_handlers.TryGetValue(commandId, out var existsHandler))
            {
                return (ICommandHandler<TCommand>) existsHandler;
            }

            throw Error.NotFound($"Command handler for command '{typeof(TCommand).Name}' is not registered");
        }
    }
}