using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class PolymorphicCommandHandler : ICommandHandler<IPolymorphicCommand>
    {
        public bool CreateBooCalled { get; private set; }
        public bool UpdateBooCalled { get; private set; }

        public Task Handle(IPolymorphicCommand command, CancellationToken cancellationToken)
        {
            switch (command)
            {
                case CreateBoo _:
                    CreateBooCalled = true;
                    break;
                case UpdateBoo _:
                    UpdateBooCalled = true;
                    break;
            }
            
            return Task.CompletedTask;
        }
    }

    public interface IPolymorphicCommand
    {
    }
}