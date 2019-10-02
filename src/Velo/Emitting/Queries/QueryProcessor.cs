namespace Velo.Emitting.Queries
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
            var context = new HandlerContext<TQuery>((TQuery) query);
            return _handler.Execute(context);
        }
    }
}