using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public sealed class PreProcessor : ICommandPreProcessor<Command>
    {
        public Task PreProcess(Command command, CancellationToken cancellationToken)
        {
            command.PreProcessed = true;
            return Task.CompletedTask;
        }
    }
}