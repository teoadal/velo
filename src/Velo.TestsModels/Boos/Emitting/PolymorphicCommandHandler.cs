using System.Threading;
using Velo.Emitting;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class PolymorphicCommandHandler: ICommandHandler<IPolymorphicCommand>
    {
        private readonly CreateBooHandler _createBooHandler;
        private readonly UpdateBooHandler _updateBooHandler;

        public PolymorphicCommandHandler(IBooRepository booRepository)
        {
            _createBooHandler = new CreateBooHandler(booRepository);
            _updateBooHandler = new UpdateBooHandler(booRepository);
        }

        public void Execute(HandlerContext<IPolymorphicCommand> context)
        {
            var payload = context.Payload;

            switch (payload)
            {
                case CreateBoo createBoo:
                    _createBooHandler.Execute(new HandlerContext<CreateBoo>(createBoo));
                    break;
                case UpdateBoo updateBoo:
                    _updateBooHandler
                        .ExecuteAsync(new HandlerContext<UpdateBoo>(updateBoo), CancellationToken.None)
                        .GetAwaiter().GetResult();
                    break;
            }
        }
    }

    public interface IPolymorphicCommand: ICommand
    {
        
    }
}