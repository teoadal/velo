using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Queries
{
    internal interface IQueryProcessor
    {
    }

    internal interface IQueryProcessor<TResult> : IQueryProcessor
    {
        Task<TResult> ExecuteAsync(IQuery<TResult> query, CancellationToken cancellationToken);
    }
}