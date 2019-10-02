using System;
using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.Emitting.Queries
{
    internal sealed class QueryProcessorsCollection
    {
        private readonly Dictionary<(int, int), IQueryProcessor> _processors;

        public QueryProcessorsCollection(DependencyContainer container)
        {
            _processors = CollectProcessors(container);
        }

        public IQueryHandler<TQuery, TResult> GetHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            var queryId = Typeof<TQuery>.Id;
            var resultId = Typeof<TResult>.Id;

            var processorKey = (queryId, resultId);

            // ReSharper disable once InvertIf
            if (_processors.TryGetValue(processorKey, out var processor))
            {
                return (IQueryHandler<TQuery, TResult>) processor.Handler;
            }

            throw Error.NotFound($"Handler for query '{typeof(TQuery).Name}' " +
                                 $"with result '{typeof(TResult).Name}' is not registered");
        }

        public IQueryProcessor<TResult> GetProcessor<TResult>(IQuery<TResult> query)
        {
            var queryId = Typeof.GetTypeId(query.GetType());
            var resultId = Typeof<TResult>.Id;

            var processorKey = (queryId, resultId);

            // ReSharper disable once InvertIf
            if (_processors.TryGetValue(processorKey, out var processor))
            {
                return (IQueryProcessor<TResult>) processor;
            }

            throw Error.NotFound($"Handler for query '{query.GetType().Name}' " +
                                 $"with result '{typeof(TResult).Name}' is not registered");
        }

        private static Dictionary<(int, int), IQueryProcessor> CollectProcessors(DependencyContainer container)
        {
            var handlers = container.Resolve<IQueryHandler[]>();
            var queryHandlerGenericType = typeof(IQueryHandler<,>);

            var processors = new Dictionary<(int, int), IQueryProcessor>(handlers.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var handlerTypes = ReflectionUtils.GetGenericInterfaceParameters(handlerType, queryHandlerGenericType);

                var processorType = typeof(QueryProcessor<,>).MakeGenericType(handlerTypes);
                var processor = (IQueryProcessor) Activator.CreateInstance(processorType, handler);

                var queryTypeId = Typeof.GetTypeId(handlerTypes[0]);
                var resultTypeId = Typeof.GetTypeId(handlerTypes[1]);
                var processorKey = (queryTypeId, resultTypeId);

                processors.Add(processorKey, processor);
            }

            return processors;
        }
    }
}