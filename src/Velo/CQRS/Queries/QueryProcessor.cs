namespace Velo.CQRS.Queries
{
    internal sealed class QueryProcessor<TQuery, TResult> : IQueryProcessor<TResult>
        where TQuery : IQuery<TResult>
    {
        public IQueryHandler Handler => _handler;

        private readonly IQueryHandler<TQuery, TResult> _handler;

        public QueryProcessor(IQueryHandler<TQuery, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Execute(IQuery<TResult> query)
        {
            return _handler.Execute((TQuery) query);
        }
    }
}