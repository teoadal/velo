using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Velo.CQRS.Queries.Pipeline
{
    internal sealed class QuerySimplePipeline<TQuery, TResult> : IQueryPipeline<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private IQueryProcessor<TQuery, TResult> _processor;

        public QuerySimplePipeline(IQueryProcessor<TQuery, TResult> processor)
        {
            _processor = processor;
        }

        public Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
        {
            return _processor.Process(query, cancellationToken);
        }

        Task<TResult> IQueryPipeline<TResult>.GetResponse(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return GetResponse((TQuery) query, cancellationToken);
        }

        public void Dispose()
        {
            _processor = null!;
        }
    }
}

#nullable restore