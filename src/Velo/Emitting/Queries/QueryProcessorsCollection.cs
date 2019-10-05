using System;
using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.Emitting.Queries
{
    internal sealed class QueryProcessorsCollection
    {
        private static readonly Type AsyncHandlerGenericType = typeof(IAsyncQueryHandler<,>);
        private static readonly Type AsyncProcessorGenericType = typeof(AsyncQueryProcessor<,>);
        private static readonly Type HandlerGenericType = typeof(IQueryHandler<,>);
        private static readonly Type ProcessorGenericType = typeof(QueryProcessor<,>);

        private readonly Dictionary<(int, int), IQueryProcessor> _processors;

        public QueryProcessorsCollection(DependencyContainer container)
        {
            _processors = CollectProcessors(container);
        }

        public IQueryProcessor GetProcessor<TResult>(IQuery<TResult> query)
        {
            var queryId = Typeof.GetTypeId(query.GetType());
            var resultId = Typeof<TResult>.Id;

            var processorKey = (queryId, resultId);

            if (_processors.TryGetValue(processorKey, out var processor))
            {
                return processor;
            }

            throw Error.NotFound($"Handler for query '{query.GetType().Name}' " +
                                 $"with result '{typeof(TResult).Name}' is not registered");
        }

        private static Dictionary<(int, int), IQueryProcessor> CollectProcessors(DependencyContainer container)
        {
            var handlers = container.Resolve<IQueryHandler[]>();
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

        private static IQueryProcessor CreateProcessor(Type handlerType, IQueryHandler handler, out Type[] genericTypes)
        {
            var isAsyncHandler = ReflectionUtils.IsGenericInterfaceImplementation(handlerType, AsyncHandlerGenericType);

            genericTypes = isAsyncHandler
                ? ReflectionUtils.GetGenericInterfaceParameters(handlerType, AsyncHandlerGenericType)
                : ReflectionUtils.GetGenericInterfaceParameters(handlerType, HandlerGenericType);

            var processorType = isAsyncHandler
                ? AsyncProcessorGenericType.MakeGenericType(genericTypes)
                : ProcessorGenericType.MakeGenericType(genericTypes);

            return (IQueryProcessor) Activator.CreateInstance(processorType, handler);
        }
    }
}