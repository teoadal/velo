namespace Velo.Emitting.Queries
{
    public interface IQueryHandler<TQuery, out TResult> : IQueryHandler
        where TQuery : IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }

    public interface IQueryHandler
    {
    }
}