using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryPreProcessor<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task PreProcess(TQuery query, CancellationToken cancellationToken);
    }
}