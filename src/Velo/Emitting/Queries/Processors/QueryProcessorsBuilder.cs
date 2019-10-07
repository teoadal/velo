using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Emitting.Queries.Processors
{
    internal sealed class QueryProcessorsBuilder
    {
        private readonly Type _asyncHandlerGenericType;
        private readonly Type _asyncProcessorGenericType;
        private readonly Type _handlerGenericType;
        private readonly Type _processorGenericType;

        private readonly Type[] _handlerContracts;
        
        private readonly IServiceProvider _container;
        
        public QueryProcessorsBuilder(IServiceProvider container)
        {
            _container = container;
            
            _asyncHandlerGenericType = typeof(IAsyncQueryHandler<,>);
            _asyncProcessorGenericType = typeof(AsyncQueryProcessor<,>);
            _handlerGenericType = typeof(IQueryHandler<,>);
            _processorGenericType = typeof(QueryProcessor<,>);
            
            _handlerContracts = new[] {_asyncHandlerGenericType, _handlerGenericType};
        }
        
        public Dictionary<(int, int), IQueryProcessor> CollectProcessors()
        {
            var handlers = (IQueryHandler[]) _container.GetService(typeof(IQueryHandler[]));
            var processors = new Dictionary<(int, int), IQueryProcessor>(handlers.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var foundContracts = ReflectionUtils.GetInheritedGenericInterfaces(handlerType, _handlerContracts);

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var j = 0; j < foundContracts.Length; j++)
                {
                    var processor = CreateProcessor(foundContracts[j], handler, out var contractTypes);

                    var queryTypeId = Typeof.GetTypeId(contractTypes[0]);
                    var resultTypeId = Typeof.GetTypeId(contractTypes[1]);
                    var processorKey = (queryTypeId, resultTypeId);

                    processors.Add(processorKey, processor);
                }
            }

            return processors;
        }

        private IQueryProcessor CreateProcessor(Type contract, IQueryHandler handler, out Type[] contractTypes)
        {
            var isAsyncHandler = ReflectionUtils.IsGenericInterfaceImplementation(contract, _asyncHandlerGenericType);

            contractTypes = isAsyncHandler
                ? ReflectionUtils.GetGenericInterfaceParameters(contract, _asyncHandlerGenericType)
                : ReflectionUtils.GetGenericInterfaceParameters(contract, _handlerGenericType);

            var processorType = isAsyncHandler
                ? _asyncProcessorGenericType.MakeGenericType(contractTypes)
                : _processorGenericType.MakeGenericType(contractTypes);

            return (IQueryProcessor) Activator.CreateInstance(processorType, handler);
        }
    }
}