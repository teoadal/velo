using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Commands
{
    internal interface ICommandProcessor
    {
        Task ProcessStoredAsync(CancellationToken cancellationToken);
    }
}