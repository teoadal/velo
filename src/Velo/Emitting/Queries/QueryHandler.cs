using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Queries
{
    public abstract class QueryHandler<TQuery, TResult>: IQueryHandler<TQuery, TResult>
        where TQuery: IQuery<TResult>
    {
        public Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken)
        {
            var result = Execute(query);
            return Task.FromResult(result);
        }

        protected abstract TResult Execute(TQuery query);
    }
}