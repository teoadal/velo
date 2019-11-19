using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryProcessor<in TRequest, TResult> : IQueryProcessor
        where TRequest : IQuery<TResult>
    {
        Task<TResult> Process(TRequest request, CancellationToken cancellationToken);
    }

    public interface IQueryProcessor
    {
    }
}