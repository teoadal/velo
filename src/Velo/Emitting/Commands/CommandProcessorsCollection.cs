using System;
using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.Emitting.Commands
{
    internal sealed class CommandProcessorsCollection
    {
        private static readonly Type CommandHandlerGenericType = typeof(ICommandHandler<>);
        private static readonly Type CommandProcessorGenericType = typeof(CommandProcessor<>);
        
        private readonly Dictionary<int, ICommandProcessor> _processors;

        public CommandProcessorsCollection(DependencyContainer container)
        {
            var handlersByCommandType = CollectHandlersByCommandType(container);
            
            var processors = new Dictionary<int, ICommandProcessor>(handlersByCommandType.Count);
            foreach (var pair in handlersByCommandType)
            {
                var commandType = pair.Key;
                var processor = BuildProcessor(commandType, pair.Value);
                
                processors.Add(Typeof.GetTypeId(commandType), processor);
            }

            _processors = processors;
        }

        public ICommandProcessor<TCommand> GetProcessor<TCommand>() where TCommand : ICommand
        {
            var commandId = Typeof<TCommand>.Id;

            if (_processors.TryGetValue(commandId, out var existsHandler))
            {
                return (ICommandProcessor<TCommand>) existsHandler;
            }

            throw Error.NotFound($"Command handler for command '{typeof(TCommand).Name}' is not registered");
        }

        private static ICommandProcessor BuildProcessor(Type commandType, List<ICommandHandler> handlers)
        {
            var commandHandlerType = CommandHandlerGenericType.MakeGenericType(commandType);
            var handlersArray = ReflectionUtils.CreateArray(commandHandlerType, handlers);

            var processorType = CommandProcessorGenericType.MakeGenericType(commandType);
            var processorConstructor = ReflectionUtils.GetConstructor(processorType);
            
            var processor = (ICommandProcessor) processorConstructor.Invoke(new object[] {handlersArray});
            return processor;
        }
        
        private static Dictionary<Type, List<ICommandHandler>> CollectHandlersByCommandType(
            DependencyContainer container)
        {
            var handlers = container.Resolve<ICommandHandler[]>();

            var handlersByCommandType = new Dictionary<Type, List<ICommandHandler>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];

                var handlerType = handler.GetType();
                var commandType = ReflectionUtils.GetGenericInterfaceParameters(handlerType, CommandHandlerGenericType)[0];

                if (!handlersByCommandType.TryGetValue(commandType, out var handlerGroup))
                {
                    handlerGroup = new List<ICommandHandler>();
                    handlersByCommandType.Add(commandType, handlerGroup);
                }

                handlerGroup.Add(handler);
            }

            return handlersByCommandType;
        }
    }
}