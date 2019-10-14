using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryRouter
    {
        public static readonly Type HandlerType = typeof(IQueryHandler<,>);
        private static readonly Type ProcessorType = typeof(QueryProcessor<,>);
        private static readonly Type RequestType = typeof(IQuery<>);

        private readonly Func<Type, IQueryProcessor> _buildProcessor;
        private readonly ConcurrentDictionary<Type, IQueryProcessor> _processors;

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
    }
}