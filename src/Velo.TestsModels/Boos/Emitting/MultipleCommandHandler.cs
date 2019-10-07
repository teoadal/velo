using Velo.Emitting;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public sealed class MultipleCommandHandler : ICommandHandler<CreateBoo>, ICommandHandler<UpdateBoo>
    {
        public bool CreateBooCalled { get; private set; }
        public bool UpdateBooCalled { get; private set; }
        
        public void Execute(HandlerContext<CreateBoo> context)
        {
            CreateBooCalled = true;
        }

        public void Execute(HandlerContext<UpdateBoo> context)
        {
            UpdateBooCalled = true;
        }
    }
}