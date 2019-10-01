using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryHandlerCollection
    {
        private readonly Dictionary<int, Dictionary<int, IQueryHandler>> _handlers;

        public QueryHandlerCollection(DependencyContainer container)
        {
            var handlers = container.Resolve<IQueryHandler[]>();
            var queryHandlerGenericType = typeof(IQueryHandler<,>);

            _handlers = new Dictionary<int, Dictionary<int, IQueryHandler>>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                var handlerType = handler.GetType();

                var handlerTypes = ReflectionUtils.GetGenericInterfaceParameters(handlerType, queryHandlerGenericType);
                var queryTypeId = Typeof.GetTypeId(handlerTypes[0]);
                var resultTypeId = Typeof.GetTypeId(handlerTypes[1]);

                if (!_handlers.TryGetValue(queryTypeId, out var resultHandlers))
                {
                    resultHandlers = new Dictionary<int, IQueryHandler>();
                    _handlers.Add(queryTypeId, resultHandlers);
                }

                resultHandlers.Add(resultTypeId, handler);
            }
        }

        public IQueryHandler<TQuery, TResult> GetHandler<TQuery, TResult>() where TQuery : IQuery<TResult>
        {
            var queryId = Typeof<TQuery>.Id;
            var resultId = Typeof<TResult>.Id;

            // ReSharper disable once InvertIf
            if (_handlers.TryGetValue(queryId, out var resultHandlers))
            {
                if (resultHandlers.TryGetValue(resultId, out var handler))
                {
                    return (IQueryHandler<TQuery, TResult>) handler;
                }
            }

            throw Error.NotFound($"Handler for query '{typeof(TQuery).Name}' " +
                                 $"with result '{typeof(TQuery).Name}' is not registered");
        }
    }
}