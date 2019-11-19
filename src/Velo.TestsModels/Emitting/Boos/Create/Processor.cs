using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class Processor : ICommandProcessor<Command>
    {
        private readonly IBooRepository _repository;

        public Processor(IBooRepository repository)
        {
            _repository = repository;
        }

        public Task Process(Command command, CancellationToken cancellationToken)
        {
            _repository.AddElement(new Boo
            {
                Id = command.Id,
                Bool = command.Bool,
                Int = command.Int
            });

            return Task.CompletedTask;
        }
    }
}