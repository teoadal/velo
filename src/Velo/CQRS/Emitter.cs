using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.DependencyInjection;
using Velo.Threading;
using Velo.Utils;

namespace Velo.CQRS
{
    internal sealed class Emitter : IEmitter, IDisposable
    {
        private IServiceProvider _services;
        
        private bool _disposed;

        internal Emitter(IServiceProvider services)
        {
            _services = services;
        }

        public Task<TResult> Ask<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var handlerType = Types.GetQueryPipelineType(query.GetType());
            var handler = (IQueryPipeline<TResult>) _services.GetRequired(handlerType);

            return handler.GetResponse(query, cancellationToken);
        }

        public Task<TResult> Ask<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            EnsureNotDisposed();

            var handler = _services.GetRequired<IQueryPipeline<TQuery, TResult>>();
            return handler.GetResponse(query, cancellationToken);
        }

        public Task Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            EnsureNotDisposed();

            var executor = _services.GetRequired<ICommandPipeline<TCommand>>();
            return executor.Execute(command, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            EnsureNotDisposed();

            var publisherType = Typeof<INotificationPipeline<TNotification>>.Raw;
            var publisher = (INotificationPipeline<TNotification>) _services.GetService(publisherType);
            return publisher?.Publish(notification, cancellationToken) ?? TaskUtils.CompletedTask;
        }

        public Task Send(ICommand command, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var executorType = Types.GetCommandPipelineType(command.GetType());
            var executor = (ICommandPipeline) _services.GetService(executorType);

            return executor.Send(command, cancellationToken);
        }

        public Task Send(INotification notification, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var publisherType = Types.GetNotificationPipelineType(notification.GetType());
            var publisher = (INotificationPipeline) _services.GetService(publisherType);

            return publisher?.Publish(notification, cancellationToken) ?? TaskUtils.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));
        }

        public void Dispose()
        {
            _services = null!;
            _disposed = true;
        }
    }
}