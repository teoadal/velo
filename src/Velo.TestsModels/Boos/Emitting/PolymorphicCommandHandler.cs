using Velo.Emitting;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class PolymorphicCommandHandler : ICommandHandler<IPolymorphicCommand>
    {
        public bool ExecuteWithCreateBooCalled { get; private set; }
        public bool ExecuteWithUpdateBooCalled { get; private set; }

        public void Execute(HandlerContext<IPolymorphicCommand> context)
        {
            var payload = context.Payload;

            switch (payload)
            {
                case CreateBoo _:
                    ExecuteWithCreateBooCalled = true;
                    break;
                case UpdateBoo _:
                    ExecuteWithUpdateBooCalled = true;
                    break;
            }
        }
    }

    public interface IPolymorphicCommand : ICommand
    {
    }
}