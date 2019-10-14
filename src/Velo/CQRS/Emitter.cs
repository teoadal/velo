using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;

namespace Velo.CQRS
{
    public sealed class Emitter
    {
        private readonly CommandRouter _commandRouter;
        private readonly QueryRouter _queryRouter;
        private readonly DependencyProvider _scope;

        internal Emitter(DependencyProvider scope, CommandRouter commandRouter, QueryRouter queryRouter)
        {
            _scope = scope;

            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
        }

        public Task Publish<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        {
            var processor = _commandRouter.GetProcessor<TCommand>();
            return processor.Execute(_scope, command, cancellationToken);
        }

        public Task<TResult> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var processor = _queryRouter.GetProcessor(query);
            return processor.Execute(_scope, query, cancellationToken);
        }
    }
}