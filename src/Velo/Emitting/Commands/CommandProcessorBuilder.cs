using System;
using System.Collections.Generic;
using Velo.Ordering;
using Velo.Utils;

namespace Velo.Emitting.Commands
{
    internal sealed class CommandProcessorBuilder
    {
        private readonly Type _asyncHandlerGenericType = typeof(IAsyncCommandHandler<>);
        private readonly Type _handlerGenericType = typeof(ICommandHandler<>);
        private readonly Type _asyncProcessorGenericType = typeof(AsyncCommandProcessor<>);
        private readonly Type _mixedProcessorGenericType = typeof(AsyncCommandProcessor<>);
        private readonly Type _syncProcessorGenericType = typeof(SyncCommandProcessor<>);

        private IServiceProvider _container;

        public CommandProcessorBuilder(IServiceProvider container)
        {
            _container = container;

            _asyncHandlerGenericType = typeof(IAsyncCommandHandler<>);
            _handlerGenericType = typeof(ICommandHandler<>);
            _asyncProcessorGenericType = typeof(AsyncCommandProcessor<>);
            _mixedProcessorGenericType = typeof(AsyncCommandProcessor<>);
            _syncProcessorGenericType = typeof(SyncCommandProcessor<>);
        }

        public Dictionary<int, ICommandProcessor> CollectProcessors()
        {
            var handlers = (ICommandHandler[]) _container.GetService(typeof(ICommandHandler[]));

            var handlersByCommandType = new Dictionary<Type, List<ICommandHandler>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var isAsyncHandler =
                    ReflectionUtils.IsGenericInterfaceImplementation(handlerType, _asyncHandlerGenericType);

                var commandType = isAsyncHandler
                    ? ReflectionUtils.GetGenericInterfaceParameters(handlerType, _asyncHandlerGenericType)[0]
                    : ReflectionUtils.GetGenericInterfaceParameters(handlerType, _handlerGenericType)[0];

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

        private ICommandProcessor CreateProcessor(Type commandType, List<ICommandHandler> handlers)
        {
            var processorGenericType = DefineGenericProcessorType(handlers);
            var processorType = processorGenericType.MakeGenericType(commandType);
            var processorConstructor = ReflectionUtils.GetConstructor(processorType);

            var constructorParameters = new object[] {handlers};
            var processor = (ICommandProcessor) processorConstructor.Invoke(constructorParameters);
            return processor;
        }

        private Type DefineGenericProcessorType(List<ICommandHandler> handlers)
        {
            var asyncCount = 0;
            
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var handler in handlers)
            {
                if (ReflectionUtils.IsGenericInterfaceImplementation(handler.GetType(), _asyncHandlerGenericType))
                {
                    asyncCount++;
                }
            }

            if (asyncCount == 0) return _syncProcessorGenericType;

            var allAsync = asyncCount == handlers.Count;
            return allAsync
                ? _asyncProcessorGenericType
                : _mixedProcessorGenericType;
        }
    }
}