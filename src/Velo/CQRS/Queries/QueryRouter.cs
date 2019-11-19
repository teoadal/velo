using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryRouter : IDisposable
    {
        public static readonly Type[] ProcessorTypes = {typeof(IQueryProcessor<,>)};
        
        private static readonly Type HandlerType = typeof(QueryHandler<,>);
        private static readonly Type QueryType = typeof(IQuery<>);

        private Func<Type, IQueryHandler> _buildHandler;
        private ConcurrentDictionary<Type, IQueryHandler> _handlers;

        public QueryRouter()
        {
            _buildHandler = BuildProcessor;
            _handlers = new ConcurrentDictionary<Type, IQueryHandler>();
        }

        public IQueryHandler<TResult> GetHandler<TResult>(IQuery<TResult> query)
        {
            var queryType = query.GetType();
            var handler = _handlers.GetOrAdd(queryType, _buildHandler);
            
            return (IQueryHandler<TResult>) handler;
        }

        public QueryHandler<TQuery, TResult> GetHandler<TQuery, TResult>()
            where TQuery: IQuery<TResult>
        {
            var queryType = Typeof<TQuery>.Raw;
            var handler = _handlers.GetOrAdd(queryType, _buildHandler);
            
            return (QueryHandler<TQuery, TResult>) handler;
        }
        
        private static IQueryHandler BuildProcessor(Type queryType)
        {
            var resultType = ReflectionUtils.GetGenericInterfaceParameters(queryType, QueryType)[0];
            var handlerType = HandlerType.MakeGenericType(queryType, resultType);
            
            return (IQueryHandler) Activator.CreateInstance(handlerType);
        }

        public void Dispose()
        {
            _buildHandler = null!;
            
            CollectionUtils.DisposeValuesIfDisposable(_handlers);
            _handlers.Clear();
            _handlers = null!;
        }
    }
}