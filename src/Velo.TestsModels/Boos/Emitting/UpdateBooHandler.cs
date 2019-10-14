using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class UpdateBooHandler : ICommandHandler<UpdateBoo>
    {
        private readonly IBooRepository _booRepository;

        public UpdateBooHandler(IBooRepository booRepository)
        {
            _booRepository = booRepository;
        }

        public Task Handle(UpdateBoo command, CancellationToken cancellationToken)
        {
            return Task.Run(() => _booRepository.UpdateElement(command.Id, boo =>
            {
                boo.Bool = command.Bool;
                boo.Int = command.Int;
            }), cancellationToken);
        }
    }
}