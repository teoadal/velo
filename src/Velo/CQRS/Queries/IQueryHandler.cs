using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryHandler<in TRequest, TResult> : IQueryHandler
        where TRequest : IQuery<TResult>
    {
        Task<TResult> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IQueryHandler
    {
    }
}