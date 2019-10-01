using System;
using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryProcessorsCollection
    {
        private readonly Dictionary<int, Dictionary<int, IQueryProcessor>> _processors;

        public QueryProcessorsCollection(DependencyContainer container)
        {
            var handlers = container.Resolve<IQueryHandler[]>();
            var queryHandlerGenericType = typeof(IQueryHandler<,>);

            _processors = new Dictionary<int, Dictionary<int, IQueryProcessor>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var handlerTypes = ReflectionUtils.GetGenericInterfaceParameters(handlerType, queryHandlerGenericType);
                var queryTypeId = Typeof.GetTypeId(handlerTypes[0]);
                var resultTypeId = Typeof.GetTypeId(handlerTypes[1]);

                if (!_processors.TryGetValue(queryTypeId, out var resultHandlers))
                {
                    resultHandlers = new Dictionary<int, IQueryProcessor>();
                    _processors.Add(queryTypeId, resultHandlers);
                }

                var processorType = typeof(QueryProcessor<,>).MakeGenericType(handlerTypes);
                var processor = (IQueryProcessor) Activator.CreateInstance(processorType, handler);
                resultHandlers.Add(resultTypeId, processor);
            }
        }

        public IQueryHandler<TQuery, TResult> GetHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            var queryId = Typeof<TQuery>.Id;
            var resultId = Typeof<TResult>.Id;

            // ReSharper disable once InvertIf
            if (_processors.TryGetValue(queryId, out var resultProcessors))
            {
                if (resultProcessors.TryGetValue(resultId, out var handler))
                {
                    return (IQueryHandler<TQuery, TResult>) handler.Handler;
                }
            }

            throw Error.NotFound($"Handler for query '{typeof(TQuery).Name}' " +
                                 $"with result '{typeof(TResult).Name}' is not registered");
        }
        
        public IQueryProcessor<TResult> GetProcessor<TResult>(IQuery<TResult> query)
        {
            var queryId = Typeof.GetTypeId(query.GetType());
            var resultId = Typeof<TResult>.Id;

            // ReSharper disable once InvertIf
            if (_processors.TryGetValue(queryId, out var resultHandlers))
            {
                if (resultHandlers.TryGetValue(resultId, out var handler))
                {
                    return (IQueryProcessor<TResult>) handler;
                }
            }

            throw Error.NotFound($"Handler for query '{query.GetType().Name}' " +
                                 $"with result '{typeof(TResult).Name}' is not registered");
        }
    }
}