using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public class GetBooIntHandler : IQueryHandler<GetBooInt, int>
    {
        private readonly IBooRepository _repository;

        public GetBooIntHandler(IBooRepository repository)
        {
            _repository = repository;
        }

        public Task<int> Handle(GetBooInt request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetElement(request.Id).Int);
        }
    }
}