using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Queries.Processors
{
    internal interface IAsyncQueryProcessor<TResult> : IQueryProcessor
    {
        Task<TResult> ExecuteAsync(IQuery<TResult> query, CancellationToken cancellationToken);
    }

    internal class AsyncQueryProcessor<TQuery, TResult> : IAsyncQueryProcessor<TResult>, IQueryProcessor<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IAsyncQueryHandler<TQuery, TResult> _handler;

        public AsyncQueryProcessor(IAsyncQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Execute(IQuery<TResult> query)
        {
            return ExecuteAsync(query, CancellationToken.None).GetAwaiter().GetResult();
        }

        public Task<TResult> ExecuteAsync(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return _handler.ExecuteAsync((TQuery) query, cancellationToken);
        }
    }
}