using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    public interface ICommandBehaviour<in TCommand>
        where TCommand : ICommand
    {
        Task Execute(TCommand command, Func<Task> next, CancellationToken cancellationToken);
    }
}