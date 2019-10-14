using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public sealed class MultipleCommandHandler : ICommandHandler<CreateBoo>, ICommandHandler<UpdateBoo>
    {
        public bool CreateBooCalled { get; private set; }
        public bool UpdateBooCalled { get; private set; }
        
        public Task Handle(CreateBoo command, CancellationToken cancellationToken)
        {
            CreateBooCalled = true;
            return Task.CompletedTask;
        }

        public Task Handle(UpdateBoo command, CancellationToken cancellationToken)
        {
            UpdateBooCalled = true;
            return Task.CompletedTask;
        }
    }
}