using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries.Pipeline
{
    internal interface IQueryPipeline<in TQuery, TResult> : IQueryPipeline<TResult>
        where TQuery: notnull, IQuery<TResult>
    {
        Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken);
    }
    
    internal interface IQueryPipeline<TResult> : IDisposable
    {
        Task<TResult> GetResponse(IQuery<TResult> query, CancellationToken cancellationToken);
    }
}