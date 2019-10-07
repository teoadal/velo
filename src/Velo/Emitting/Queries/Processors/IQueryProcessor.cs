namespace Velo.Emitting.Queries.Processors
{
    internal interface IQueryProcessor
    {
    }

    internal interface IQueryProcessor<TResult> : IQueryProcessor
    {
        TResult Execute(IQuery<TResult> query);
    }
}