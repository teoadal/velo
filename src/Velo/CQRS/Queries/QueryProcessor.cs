using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryProcessor<TRequest, TResult> : IQueryProcessor<TResult>
        where TRequest : IQuery<TResult>
    {
        private readonly Type _handlerType;

        public QueryProcessor()
        {
            _handlerType = Typeof<IQueryHandler<TRequest, TResult>>.Raw;
        }

        public Task<TResult> Execute(IServiceProvider scope, IQuery<TResult> query,
            CancellationToken cancellationToken)
        {
            var handler = (IQueryHandler<TRequest, TResult>) scope.GetService(_handlerType);
            return handler.Handle((TRequest) query, cancellationToken);
        }
    }

    internal interface IQueryProcessor<TResult> : IQueryProcessor
    {
        Task<TResult> Execute(IServiceProvider scope, IQuery<TResult> query, CancellationToken cancellationToken);
    }

    internal interface IQueryProcessor
    {
    }
}