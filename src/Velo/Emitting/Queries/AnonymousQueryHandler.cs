using System;
using Velo.Dependencies;

namespace Velo.Emitting.Queries
{
    internal sealed class AnonymousQueryHandler<TQuery, TResult> : EmitterContext, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<EmitterContext, TQuery, TResult> _handler;

        public AnonymousQueryHandler(DependencyContainer container, Func<EmitterContext, TQuery, TResult> handler)
            : base(container)
        {
            _handler = handler;
        }

        public TResult Execute(TQuery query)
        {
            return _handler(this, query);
        }
    }
}