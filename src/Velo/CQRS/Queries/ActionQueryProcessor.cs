using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class ActionQueryProcessor<TQuery, TResult> : IQueryProcessor<TQuery, TResult>, IDisposable
        where TQuery : IQuery<TResult>
    {
        private Func<TQuery, TResult> _processor;

        private bool _disposed;

        public ActionQueryProcessor(Func<TQuery, TResult> processor)
        {
            _processor = processor;
        }

        public Task<TResult> Process(TQuery query, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(IQueryProcessor<TQuery, TResult>));
            
            var result = _processor(query);
            return Task.FromResult(result);
        }

        public void Dispose()
        {
            _processor = null!;
            _disposed = true;
        }
    }
}