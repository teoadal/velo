using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public sealed class GetBooHandler : IQueryHandler<GetBoo, Boo>
    {
        private readonly IBooRepository _repository;

        public GetBooHandler(IBooRepository repository)
        {
            _repository = repository;
        }
        
        public Task<Boo> Handle(GetBoo request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetElement(request.Id));
        }
    }
}