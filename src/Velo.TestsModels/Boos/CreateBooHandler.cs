using Velo.CQRS.Commands;

namespace Velo.TestsModels.Boos
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
            _repository.AddElement(new Boo {Id = command.Id, Bool = command.Bool, Int = command.Int});
        }
    }
}