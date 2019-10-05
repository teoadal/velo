using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Queries
{
    public interface IAsyncQueryHandler<in TQuery, TResult> : IQueryHandler
        where TQuery: IQuery<TResult>
    {
        Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
    }
}