using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;
using Velo.Utils;

namespace Velo.CQRS
{
    public sealed class Emitter : IDisposable
    {
        private CommandRouter _commandRouter;
        private QueryRouter _queryRouter;
        private NotificationRouter _notificationRouter;
        private IServiceProvider _scope;

        private bool _disposed;

        internal Emitter(IServiceProvider scope, CommandRouter commandRouter, NotificationRouter notificationRouter, QueryRouter queryRouter)
        {
            _scope = scope;

            _commandRouter = commandRouter;
            _notificationRouter = notificationRouter;
            _queryRouter = queryRouter;
        }

        public Task<TResult> Ask<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var handler = _queryRouter.GetHandler(query);
            return handler.GetResponse(_scope, query, cancellationToken);
        }

        public Task<TResult> Ask<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery: IQuery<TResult>
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var handler = _queryRouter.GetHandler<TQuery, TResult>();
            return handler.GetResponse(_scope, query, cancellationToken);
        }
        
        public Task Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var executor = _commandRouter.GetExecutor<TCommand>();
            return executor.Execute(_scope, command, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var publisher = _notificationRouter.GetPublisher<TNotification>();
            return publisher.Publish(_scope, notification, cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _commandRouter = null!;
            _queryRouter = null!;
            _notificationRouter = null!;

            _scope = null!;

            _disposed = true;
        }
    }
}