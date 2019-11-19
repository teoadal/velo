using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    public interface ICommandPostProcessor<in TCommand> : ICommandProcessor
        where TCommand : ICommand
    {
        Task PostProcess(TCommand command, CancellationToken cancellationToken);
    }
}