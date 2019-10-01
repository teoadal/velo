namespace Velo.CQRS.Queries
{
    public interface IQueryHandler<in TQuery, out TResult> : IQueryHandler
        where TQuery : IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }

    public interface IQueryHandler
    {
    }
}