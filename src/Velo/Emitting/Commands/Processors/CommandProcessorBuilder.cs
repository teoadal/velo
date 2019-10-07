using System;
using System.Collections.Generic;
using Velo.Ordering;
using Velo.Utils;

namespace Velo.Emitting.Commands.Processors
{
    internal sealed class CommandProcessorBuilder
    {
        private readonly Type _asyncHandlerGenericType;
        private readonly Type _handlerGenericType;
        private readonly Type _asyncProcessorGenericType;
        private readonly Type _mixedProcessorGenericType;
        private readonly Type _syncProcessorGenericType;

        private readonly Type[] _handlerContracts;
        
        private readonly IServiceProvider _container;

        public CommandProcessorBuilder(IServiceProvider container)
        {
            _container = container;

            _asyncHandlerGenericType = typeof(IAsyncCommandHandler<>);
            _handlerGenericType = typeof(ICommandHandler<>);
            _asyncProcessorGenericType = typeof(AsyncCommandProcessor<>);
            _mixedProcessorGenericType = typeof(AsyncCommandProcessor<>);
            _syncProcessorGenericType = typeof(SyncCommandProcessor<>);

            _handlerContracts = new[] {_asyncHandlerGenericType, _handlerGenericType};
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

                var foundContracts = ReflectionUtils.GetInheritedGenericInterfaces(handlerType, _handlerContracts);

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var j = 0; j < foundContracts.Length; j++)
                {
                    var commandType = DefineCommandType(foundContracts[j]);
                    if (!handlersByCommandType.TryGetValue(commandType, out var handlerGroup))
                    {
                        handlerGroup = new List<ICommandHandler>();
                        handlersByCommandType.Add(commandType, handlerGroup);
                    }

                    handlerGroup.Add(handler);
                }
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

        private Type DefineCommandType(Type contract)
        {
            var isAsync = ReflectionUtils.IsGenericInterfaceImplementation(contract, _asyncHandlerGenericType);

            return isAsync
                ? ReflectionUtils.GetGenericInterfaceParameters(contract, _asyncHandlerGenericType)[0]
                : ReflectionUtils.GetGenericInterfaceParameters(contract, _handlerGenericType)[0];
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