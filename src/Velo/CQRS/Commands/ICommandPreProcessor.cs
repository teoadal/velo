using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    public interface ICommandPreProcessor<in TCommand>
        where TCommand : ICommand
    {
        ValueTask PreProcess(TCommand command, CancellationToken cancellationToken);
    }
}