using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Emitting.Queries.Processors
{
    internal sealed class QueryProcessorsCollection
    {
        private readonly Dictionary<(int, int), IQueryProcessor> _processors;

        public QueryProcessorsCollection(IServiceProvider container)
        {
            _processors = new QueryProcessorsBuilder(container).CollectProcessors();
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
    }
}