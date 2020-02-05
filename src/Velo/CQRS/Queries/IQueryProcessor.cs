using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryProcessor<in TQuery, TResult> : IQueryProcessor
        where TQuery : IQuery<TResult>
    {
        ValueTask<TResult> Process(TQuery query, CancellationToken cancellationToken);
    }

    public interface IQueryProcessor
    {
    }
}