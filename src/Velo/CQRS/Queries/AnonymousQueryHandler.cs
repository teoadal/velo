using System;
using Velo.Dependencies;

namespace Velo.CQRS.Queries
{
    internal sealed class AnonymousQueryHandler<TQuery, TResult> : HandlerContext, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<HandlerContext, TQuery, TResult> _handler;

        public AnonymousQueryHandler(DependencyContainer container, Func<HandlerContext, TQuery, TResult> handler)
            : base(container)
        {
            _handler = handler;
        }

        public TResult Execute(TQuery payload)
        {
            return _handler(this, payload);
        }
    }
}