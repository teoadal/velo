using Velo.Emitting;
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

        public void Execute(HandlerContext<CreateBoo> context)
        {
            var payload = context.Payload;

            _repository.AddElement(new Boo
            {
                Id = payload.Id,
                Bool = payload.Bool,
                Int = payload.Int
            });
        }
    }
}