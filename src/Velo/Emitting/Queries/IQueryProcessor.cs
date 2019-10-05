namespace Velo.Emitting.Queries
{
    internal interface IQueryProcessor
    {
    }

    internal interface IQueryProcessor<TResult> : IQueryProcessor
    {
        TResult Execute(IQuery<TResult> query);
    }
}