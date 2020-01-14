using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public sealed class PreProcessor : ICommandPreProcessor<Command>
    {
        public ValueTask PreProcess(Command command, CancellationToken cancellationToken)
        {
            command.PreProcessed = true;
            return new ValueTask();
        }
    }
}