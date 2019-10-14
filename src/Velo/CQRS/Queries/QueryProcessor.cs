using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryProcessor<TRequest, TResult> : IQueryProcessor<TResult>
        where TRequest : IQuery<TResult>
    {
        public Task<TResult> Execute(DependencyProvider scope, IQuery<TResult> query,
            CancellationToken cancellationToken)
        {
            var handler = scope.GetService<IQueryHandler<TRequest, TResult>>();
            return handler.Handle((TRequest) query, cancellationToken);
        }
    }

    internal interface IQueryProcessor<TResult> : IQueryProcessor
    {
        Task<TResult> Execute(DependencyProvider scope, IQuery<TResult> query, CancellationToken cancellationToken);
    }

    internal interface IQueryProcessor
    {
    }
}