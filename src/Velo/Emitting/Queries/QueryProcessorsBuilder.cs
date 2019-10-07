using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Emitting.Queries
{
    internal sealed class QueryProcessorsBuilder
    {
        private readonly Type _asyncHandlerGenericType;
        private readonly Type _asyncProcessorGenericType;
        private readonly Type _handlerGenericType;
        private readonly Type _processorGenericType;

        private readonly IServiceProvider _container;
        
        public QueryProcessorsBuilder(IServiceProvider container)
        {
            _container = container;
            _asyncHandlerGenericType = typeof(IAsyncQueryHandler<,>);
            _asyncProcessorGenericType = typeof(AsyncQueryProcessor<,>);
            _handlerGenericType = typeof(IQueryHandler<,>);
            _processorGenericType = typeof(QueryProcessor<,>);
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

                var processor = CreateProcessor(handlerType, handler, out var genericTypes);

                var queryTypeId = Typeof.GetTypeId(genericTypes[0]);
                var resultTypeId = Typeof.GetTypeId(genericTypes[1]);
                var processorKey = (queryTypeId, resultTypeId);

                processors.Add(processorKey, processor);
            }

            return processors;
        }

        private IQueryProcessor CreateProcessor(Type handlerType, IQueryHandler handler, out Type[] genericTypes)
        {
            var isAsyncHandler = ReflectionUtils.IsGenericInterfaceImplementation(handlerType, _asyncHandlerGenericType);

            genericTypes = isAsyncHandler
                ? ReflectionUtils.GetGenericInterfaceParameters(handlerType, _asyncHandlerGenericType)
                : ReflectionUtils.GetGenericInterfaceParameters(handlerType, _handlerGenericType);

            var processorType = isAsyncHandler
                ? _asyncProcessorGenericType.MakeGenericType(genericTypes)
                : _processorGenericType.MakeGenericType(genericTypes);

            return (IQueryProcessor) Activator.CreateInstance(processorType, handler);
        }
    }
}