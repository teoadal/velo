using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;

namespace Velo.CQRS
{
    public interface IEmitter
    {
        Task<TResult> Ask<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        Task<TResult> Ask<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>;

        Task Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification;

        Task Send(ICommand command, CancellationToken cancellationToken = default);

        Task Send(INotification notification, CancellationToken cancellationToken = default);
    }
}