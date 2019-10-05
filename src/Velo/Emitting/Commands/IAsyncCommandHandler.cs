using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Commands
{
    public interface IAsyncCommandHandler<TCommand>: ICommandHandler
        where TCommand: ICommand
    {
        Task ExecuteAsync(HandlerContext<TCommand> context, CancellationToken cancellationToken);
    }
}