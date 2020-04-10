using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Velo.CQRS.Queries
{
    public interface IQueryBehaviour<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Execute(TQuery query, Func<Task<TResult>> next, CancellationToken cancellationToken);
    }
}

#nullable restore