using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryRouter : IDisposable
    {
        public static readonly Type HandlerType = typeof(IQueryHandler<,>);
        private static readonly Type ProcessorType = typeof(QueryProcessor<,>);
        private static readonly Type RequestType = typeof(IQuery<>);

        private Func<Type, IQueryProcessor> _buildProcessor;
        private ConcurrentDictionary<Type, IQueryProcessor> _processors;

        public QueryRouter()
        {
            _buildProcessor = BuildProcessor;
            _processors = new ConcurrentDictionary<Type, IQueryProcessor>();
        }

        public IQueryProcessor<TResult> GetProcessor<TResult>(IQuery<TResult> query)
        {
            var requestType = query.GetType();
            return (IQueryProcessor<TResult>) _processors.GetOrAdd(requestType, _buildProcessor);
        }

        private static IQueryProcessor BuildProcessor(Type requestType)
        {
            var queryType = ReflectionUtils.GetGenericInterfaceParameters(requestType, RequestType)[0];

            var processorType = ProcessorType.MakeGenericType(requestType, queryType);
            return (IQueryProcessor) Activator.CreateInstance(processorType);
        }

        public void Dispose()
        {
            _buildProcessor = null;
            
            CollectionUtils.DisposeValuesIfDisposable(_processors);
            _processors.Clear();
            _processors = null;
        }
    }
}