using System;
using System.Collections.Concurrent;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.Emitting.Queries
{
    internal sealed class QueryProcessorsCollection
    {
        private readonly DependencyContainer _container;
        private readonly Func<(Type, Type), IQueryProcessor> _findProcessor;
        private readonly ConcurrentDictionary<(Type, Type), IQueryProcessor> _processors;

        public QueryProcessorsCollection(DependencyContainer container)
        {
            _container = container;
            _findProcessor = FindProcessor;
            _processors = new ConcurrentDictionary<(Type, Type), IQueryProcessor>();
        }

        public IQueryProcessor GetProcessor<TResult>(IQuery<TResult> query)
        {
            var queryType = query.GetType();
            var resultType = Typeof<TResult>.Raw;

            return _processors.GetOrAdd((queryType, resultType), _findProcessor);
        }
        
        private IQueryProcessor FindProcessor((Type, Type) processorKey)
        {
            var (queryType, resultType) = processorKey;
            var processorType = typeof(QueryProcessor<,>).MakeGenericType(queryType, resultType);
            return (IQueryProcessor) Activator.CreateInstance(processorType, _container);
        }
    }
}