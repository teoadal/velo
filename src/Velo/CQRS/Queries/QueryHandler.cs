using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryHandler<TQuery, TResult> : IQueryHandler<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Type _processorType;

        public QueryHandler()
        {
            _processorType = typeof(IQueryProcessor<TQuery, TResult>);
        }

        public Task<TResult> GetResponse(IServiceProvider scope, TQuery query, CancellationToken cancellationToken)
        {
            var handler = (IQueryProcessor<TQuery, TResult>) scope.GetService(_processorType);

            return handler.Process(query, cancellationToken);
        }
        
        public Task<TResult> GetResponse(IServiceProvider scope, IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return GetResponse(scope, (TQuery) query, cancellationToken);
        }
    }

    internal interface IQueryHandler<TResult> : IQueryHandler
    {
        Task<TResult> GetResponse(IServiceProvider scope, IQuery<TResult> query, CancellationToken cancellationToken);
    }

    internal interface IQueryHandler
    {
    }
}