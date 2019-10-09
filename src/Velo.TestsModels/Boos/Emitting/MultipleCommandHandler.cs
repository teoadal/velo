using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public sealed class MultipleCommandHandler : ICommandHandler<CreateBoo>, ICommandHandler<UpdateBoo>
    {
        public bool CreateBooCalled { get; private set; }
        public bool UpdateBooCalled { get; private set; }
        
        public Task ExecuteAsync(CreateBoo context, CancellationToken cancellationToken)
        {
            CreateBooCalled = true;
            return Task.CompletedTask;
        }

        public Task ExecuteAsync(UpdateBoo context, CancellationToken cancellationToken)
        {
            UpdateBooCalled = true;
            return Task.CompletedTask;
        }
    }
}