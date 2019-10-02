namespace Velo.Emitting.Queries
{
    public interface IQueryHandler<TQuery, out TResult> : IQueryHandler
        where TQuery : IQuery<TResult>
    {
        TResult Execute(HandlerContext<TQuery> context);
    }

    public interface IQueryHandler
    {
    }
}