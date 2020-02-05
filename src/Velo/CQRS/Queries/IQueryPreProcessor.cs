using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryPreProcessor<in TQuery, TResult>
    {
        ValueTask<TResult> PreProcess(TQuery query, CancellationToken cancellationToken);
    }
}