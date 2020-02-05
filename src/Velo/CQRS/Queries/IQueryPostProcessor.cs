using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryPostProcessor<in TQuery, TResult> : IQueryProcessor
    {
        ValueTask<TResult> PostProcess(TQuery query, TResult result, CancellationToken cancellationToken);
    }
}