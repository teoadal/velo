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
    public sealed class Emitter : IDisposable
    {
        private bool _disposed;
        private IServiceProvider _scope;

        internal Emitter(IServiceProvider scope)
        {
            _scope = scope;
        }

        public ValueTask<TResult> Ask<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var handlerType = PipelineTypes.GetForQuery(query.GetType());
            var handler = (IQueryPipeline<TResult>) _scope.GetService(handlerType);

            return handler.GetResponse(query, cancellationToken);
        }

        public ValueTask<TResult> Ask<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var handler = GetService<QueryPipeline<TQuery, TResult>>();
            return handler.GetResponse(query, cancellationToken);
        }

        public ValueTask Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var executor = GetService<CommandPipeline<TCommand>>();
            return executor.Execute(command, cancellationToken);
        }

        public ValueTask Publish<TNotification>(TNotification notification,
            CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var publisher = GetService<NotificationPipeline<TNotification>>();
            return publisher.Publish(notification, cancellationToken);
        }

        public ValueTask Send(ICommand command, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var executorType = PipelineTypes.GetForCommand(command.GetType());
            var executor = (ICommandPipeline) _scope.GetService(executorType);

            return executor.Execute(command, cancellationToken);
        }

        public ValueTask Send(INotification notification, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));

            var publisherType = PipelineTypes.GetForNotification(notification.GetType());
            var publisher = (INotificationPipeline) _scope.GetService(publisherType);

            return publisher.Publish(notification, cancellationToken);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _scope = null;
            _disposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetService<T>()
        {
            return (T) _scope.GetService(Typeof<T>.Raw);
        }
    }
}