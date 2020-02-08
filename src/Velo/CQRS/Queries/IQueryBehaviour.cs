using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    public interface IQueryBehaviour<in TQuery, TResult>
        where TQuery: IQuery<TResult>
    {
        ValueTask<TResult> Execute(TQuery query, Func<ValueTask<TResult>> next, CancellationToken cancellationToken);
    }
}