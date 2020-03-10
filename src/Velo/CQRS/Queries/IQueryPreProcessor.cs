using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Velo.CQRS.Queries
{
    public interface IQueryPreProcessor<in TQuery, TResult>
        where TQuery : notnull, IQuery<TResult>
    {
        Task PreProcess(TQuery query, CancellationToken cancellationToken);
    }
}

#nullable restore