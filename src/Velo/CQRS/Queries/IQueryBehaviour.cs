using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryBehaviour<in TQuery, TResult>
        where TQuery: IQuery<TResult>
    {
        Task<TResult> Execute(TQuery query, Func<Task<TResult>> next, CancellationToken cancellationToken);
    }
}