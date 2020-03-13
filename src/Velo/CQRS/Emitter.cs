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
using Velo.Utils;

#nullable enable

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

            var handlerType = Types.GetQueryPipelineType(query.GetType());
            var handler = (IQueryPipeline<TResult>) _scope.GetService(handlerType);

            return handler.GetResponse(query, cancellationToken);
        }

        public Task<TResult> Ask<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : notnull, IQuery<TResult>
        {
            EnsureNotDisposed();

            var handler = GetRequiredService<IQueryPipeline<TQuery, TResult>>();
            return handler.GetResponse(query, cancellationToken);
        }

        public Task Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : notnull, ICommand
        {
            EnsureNotDisposed();

            var executor = GetRequiredService<ICommandPipeline<TCommand>>();
            return executor.Execute(command, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : notnull, INotification
        {
            EnsureNotDisposed();

            var publisherType = Typeof<INotificationPipeline<TNotification>>.Raw;
            var publisher = (INotificationPipeline<TNotification>) _scope.GetService(publisherType);
            return publisher?.Publish(notification, cancellationToken) ?? TaskUtils.CompletedTask;
        }

        public Task Send(ICommand command, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var executorType = Types.GetCommandPipelineType(command.GetType());
            var executor = (ICommandPipeline) _scope.GetService(executorType);

            return executor.Send(command, cancellationToken);
        }

        public Task Send(INotification notification, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();

            var publisherType = Types.GetNotificationPipelineType(notification.GetType());
            var publisher = (INotificationPipeline) _scope.GetService(publisherType);

            return publisher?.Publish(notification, cancellationToken) ?? TaskUtils.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T GetRequiredService<T>() where T : class
        {
            var dependencyType = Typeof<T>.Raw;
            var result = (T) _scope.GetService(dependencyType);

            if (result == null) throw Error.DependencyNotRegistered(dependencyType);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(Emitter));
        }

        public void Dispose()
        {
            _scope = null!;
            _disposed = true;
        }
    }
}

#nullable restore