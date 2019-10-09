using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Commands
{
    public interface ICommandHandler<in TCommand> : ICommandHandler
        where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler
    {
    }
}