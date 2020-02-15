using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands
{
    public interface ICommandPostProcessor<in TCommand>
        where TCommand : ICommand
    {
        ValueTask PostProcess(TCommand command, CancellationToken cancellationToken);
    }
}