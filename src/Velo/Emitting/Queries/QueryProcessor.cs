using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Velo.Dependencies;

namespace Velo.Emitting.Queries
{
    internal sealed class QueryProcessor<TQuery, TResult> : IQueryProcessor<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly DependencyContainer _container;
        private readonly IDependency _handlerDependency;
        private readonly Type _handlerType;

        public QueryProcessor(DependencyContainer container, IDependency handlerDependency)
        {
            _container = container;
            _handlerType = typeof(IQueryHandler<TQuery, TResult>);
            _handlerDependency = handlerDependency;
        }

        public Task<TResult> ExecuteAsync(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return ExecuteAsync((TQuery) query, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken)
        {
            var handler = (IQueryHandler<TQuery, TResult>) _handlerDependency.Resolve(_handlerType, _container);
            return handler.ExecuteAsync(query, cancellationToken);
        }
    }
}