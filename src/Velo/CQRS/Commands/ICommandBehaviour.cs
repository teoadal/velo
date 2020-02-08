using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    public interface ICommandBehaviour<in TCommand>
        where TCommand: ICommand
    {
        ValueTask Execute(TCommand command, Func<ValueTask> next, CancellationToken cancellationToken);
    }
}