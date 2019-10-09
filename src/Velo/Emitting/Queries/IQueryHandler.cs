using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Queries
{
    public interface IQueryHandler<in TQuery, TResult> : IQueryHandler
        where TQuery : IQuery<TResult>
    {
        Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken);
    }

    public interface IQueryHandler
    {
    }
}