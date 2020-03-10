using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Velo.CQRS.Queries
{
    public interface IQueryPostProcessor<in TQuery, in TResult>
        where TQuery : notnull, IQuery<TResult>
    {
        Task PostProcess(TQuery query, TResult result, CancellationToken cancellationToken);
    }
}

#nullable restore