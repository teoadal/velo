namespace Velo.Emitting.Queries
{
    internal interface IQueryProcessor<TResult> : IQueryProcessor
    {
        TResult Execute(IQuery<TResult> query);
    }

    internal interface IQueryProcessor
    {
        IQueryHandler Handler { get; }
    }
}