using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class PolymorphicCommandHandler : ICommandHandler<IPolymorphicCommand>
    {
        public bool ExecuteWithCreateBooCalled { get; private set; }
        public bool ExecuteWithUpdateBooCalled { get; private set; }

        public Task ExecuteAsync(IPolymorphicCommand command, CancellationToken cancellationToken)
        {
            switch (command)
            {
                case CreateBoo _:
                    ExecuteWithCreateBooCalled = true;
                    break;
                case UpdateBoo _:
                    ExecuteWithUpdateBooCalled = true;
                    break;
            }
            
            return Task.CompletedTask;
        }
    }

    public interface IPolymorphicCommand : ICommand
    {
    }
}