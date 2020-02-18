using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;
using Velo.TestsModels.Foos;

namespace Velo.TestsModels.Emitting.Foos.Create
{
    public class Processor : ICommandProcessor<Command>
    {
        private readonly IFooRepository _fooRepository;

        public Processor(IFooRepository fooRepository)
        {
            _fooRepository = fooRepository;
        }

        public Task Process(Command command, CancellationToken cancellationToken)
        {
            _fooRepository.AddElement(new Foo {Int = command.Id, Type = command.Type});
            return Task.CompletedTask;
        }
    }
}