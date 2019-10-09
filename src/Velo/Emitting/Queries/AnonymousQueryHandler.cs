using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Dependencies;

namespace Velo.Emitting.Queries
{
    internal sealed class AnonymousQueryHandler<TQuery, TResult> : EmitterContext, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<EmitterContext, TQuery, Task<TResult>> _asyncHandler;
        private readonly Func<EmitterContext, TQuery, TResult> _syncHandler;

        public AnonymousQueryHandler(DependencyContainer container,
            Func<EmitterContext, TQuery, Task<TResult>> asyncHandler)
            : base(container)
        {
            _asyncHandler = asyncHandler;
        }

        public AnonymousQueryHandler(DependencyContainer container, Func<EmitterContext, TQuery, TResult> syncHandler)
            : base(container)
        {
            _syncHandler = syncHandler;
        }

        public Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken)
        {
            return _asyncHandler == null
                ? Task.FromResult(_syncHandler(this, query))
                : _asyncHandler(this, query);
        }
    }
}