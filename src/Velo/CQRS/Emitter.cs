using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Pipeline;
using Velo.CQRS.Queries;
using Velo.Utils;

namespace Velo.CQRS
{
    internal sealed class Emitter : IEmitter, IDisposable
    {
        private bool _disposed;
        private IServiceProvider _scope;

        internal Emitter(IServiceProvider scope)
        {
            _scope = scope;
        }

        public Task<TResult> Ask<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var handlerType = PipelineTypes.GetForQuery(query.GetType());
            var handler = (IQueryPipeline<TResult>) _scope.GetService(handlerType);

            return handler.GetResponse(query, cancellationToken);
        }

        public Task<TResult> Ask<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            EnsureNotDisposed();

            var handler = GetService<QueryPipeline<TQuery, TResult>>();
            return handler.GetResponse(query, cancellationToken);
        }

        public Task Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            EnsureNotDisposed();

            var executor = GetService<CommandPipeline<TCommand>>();
            return executor.Execute(command, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            EnsureNotDisposed();

            var publisher = GetService<NotificationPipeline<TNotification>>();
            return publisher?.Publish(notification, cancellationToken) ?? TaskUtils.CompletedTask;
        }

        public Task Send(ICommand command, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var executorType = PipelineTypes.GetForCommand(command.GetType());
            var executor = (ICommandPipeline) _scope.GetService(executorType);

            return executor.Send(command, cancellationToken);
        }

        public Task Send(INotification notification, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var publisherType = PipelineTypes.GetForNotification(notification.GetType());
            var publisher = (INotificationPipeline) _scope.GetService(publisherType);

            return publisher?.Publish(notification, cancellationToken) ?? TaskUtils.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetService<T>()
        {
            return (T) _scope.GetService(Typeof<T>.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _scope = null;
            _disposed = true;
        }
    }
}