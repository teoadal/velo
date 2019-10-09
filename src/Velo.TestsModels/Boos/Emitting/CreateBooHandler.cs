using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Commands;

namespace Velo.TestsModels.Boos.Emitting
{
    public class CreateBooHandler : ICommandHandler<CreateBoo>
    {
        private readonly IBooRepository _repository;

        public CreateBooHandler(IBooRepository repository)
        {
            _repository = repository;
        }

        public void Execute(CreateBoo command)
        {
        }

        public Task ExecuteAsync(CreateBoo command, CancellationToken cancellationToken)
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