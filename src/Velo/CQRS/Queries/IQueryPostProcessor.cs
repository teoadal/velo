using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryPostProcessor<in TQuery, in TResult>
        where TQuery: IQuery<TResult>
    {
        ValueTask PostProcess(TQuery query, TResult result, CancellationToken cancellationToken);
    }
}