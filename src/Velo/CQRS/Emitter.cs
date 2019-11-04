using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.Utils;

namespace Velo.CQRS
{
    public sealed class Emitter : IDisposable
    {
        private CommandRouter _commandRouter;
        private QueryRouter _queryRouter;
        private IServiceProvider _scope;

        private bool _disposed;

        internal Emitter(IServiceProvider scope, CommandRouter commandRouter, QueryRouter queryRouter)
        {
            _scope = scope;

            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
        }

        public Task<TResult> Ask<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var processor = _queryRouter.GetProcessor(query);
            return processor.Execute(_scope, query, cancellationToken);
        }
        
        public Task Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var processor = _commandRouter.GetProcessor<TCommand>();
            return processor.Execute(_scope, command, cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _commandRouter = null;
            _queryRouter = null;
            _scope = null;

            _disposed = true;
        }
    }
}