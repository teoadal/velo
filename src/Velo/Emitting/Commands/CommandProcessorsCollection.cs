using System;
using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Ordering;
using Velo.Utils;

namespace Velo.Emitting.Commands
{
    internal sealed class CommandProcessorsCollection
    {
        private static readonly Type AsyncHandlerGenericType = typeof(IAsyncCommandHandler<>);
        private static readonly Type HandlerGenericType = typeof(ICommandHandler<>);
        private static readonly Type AsyncProcessorGenericType = typeof(AsyncCommandProcessor<>);
        private static readonly Type MixedProcessorGenericType = typeof(AsyncCommandProcessor<>);
        private static readonly Type SyncProcessorGenericType = typeof(SyncCommandProcessor<>);
        
        private readonly Dictionary<int, ICommandProcessor> _processors;

        public CommandProcessorsCollection(DependencyContainer container)
        {
            _processors = CollectProcessors(container);
        }

        public ICommandProcessor GetProcessor<TCommand>() where TCommand : ICommand
        {
            var commandId = Typeof<TCommand>.Id;

            if (_processors.TryGetValue(commandId, out var existsHandler))
            {
                return existsHandler;
            }

            throw Error.NotFound($"Command handler for command '{typeof(TCommand).Name}' is not registered");
        }

        private static Dictionary<int, ICommandProcessor> CollectProcessors(DependencyContainer container)
        {
            var handlers = container.Resolve<ICommandHandler[]>();

            var handlersByCommandType = new Dictionary<Type, List<ICommandHandler>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var isAsyncHandler =
                    ReflectionUtils.IsGenericInterfaceImplementation(handlerType, AsyncHandlerGenericType);

                var commandType = isAsyncHandler
                    ? ReflectionUtils.GetGenericInterfaceParameters(handlerType, AsyncHandlerGenericType)[0]
                    : ReflectionUtils.GetGenericInterfaceParameters(handlerType, HandlerGenericType)[0];

                if (!handlersByCommandType.TryGetValue(commandType, out var handlerGroup))
                {
                    handlerGroup = new List<ICommandHandler>();
                    handlersByCommandType.Add(commandType, handlerGroup);
                }

                handlerGroup.Add(handler);
            }

            var commandHandlerComparer = new OrderAttributeComparer<ICommandHandler>();
            var processors = new Dictionary<int, ICommandProcessor>(handlersByCommandType.Count);
            foreach (var pair in handlersByCommandType)
            {
                var commandType = pair.Key;
                var commandHandlers = pair.Value;
                
                commandHandlers.Sort(commandHandlerComparer);
                
                var processor = CreateProcessor(commandType, commandHandlers);
                processors.Add(Typeof.GetTypeId(commandType), processor);
            }

            return processors;
        }

        private static ICommandProcessor CreateProcessor(Type commandType, List<ICommandHandler> handlers)
        {
            var processorGenericType = DefineGenericProcessorType(handlers);
            var processorType = processorGenericType.MakeGenericType(commandType);
            var processorConstructor = ReflectionUtils.GetConstructor(processorType);

            var constructorParameters = new object[] {handlers};
            var processor = (ICommandProcessor) processorConstructor.Invoke(constructorParameters);
            return processor;
        }

        private static Type DefineGenericProcessorType(List<ICommandHandler> handlers)
        {
            var asyncCount = 0;
            foreach (var handler in handlers)
            {
                if (ReflectionUtils.IsGenericInterfaceImplementation(handler.GetType(), AsyncHandlerGenericType))
                {
                    asyncCount++;
                }
            }
            
            if (asyncCount == 0) return SyncProcessorGenericType;
            
            var allAsync = asyncCount == handlers.Count;
            return allAsync 
                ? AsyncProcessorGenericType
                : MixedProcessorGenericType;
        }
    }
}