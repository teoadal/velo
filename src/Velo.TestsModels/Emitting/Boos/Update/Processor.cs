using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Update
{
    public class Processor : ICommandProcessor<Command>
    {
        private readonly IBooRepository _booRepository;

        public Processor(IBooRepository booRepository)
        {
            _booRepository = booRepository;
        }

        public Task Process(Command command, CancellationToken cancellationToken)
        {
            return Task.Run(() => _booRepository.UpdateElement(command.Id, boo =>
            {
                boo.Bool = command.Bool;
                boo.Int = command.Int;
            }), cancellationToken);
        }
    }
}