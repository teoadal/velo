using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Queries.Processors
{
    internal sealed class QueryProcessor<TQuery, TResult> : IQueryProcessor<TResult>, IAsyncQueryProcessor<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;

        public QueryProcessor(IQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Execute(IQuery<TResult> query)
        {
            return _handler.Execute((TQuery) query);
        }

        public Task<TResult> ExecuteAsync(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(query));
        }
    }
}