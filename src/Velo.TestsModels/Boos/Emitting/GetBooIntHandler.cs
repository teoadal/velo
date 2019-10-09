using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public class GetBooIntHandler : IQueryHandler<GetBooInt, int>
    {
        private readonly IBooRepository _repository;

        public GetBooIntHandler(IBooRepository repository)
        {
            _repository = repository;
        }

        public Task<int> ExecuteAsync(GetBooInt query, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetElement(query.Id).Int);
        }
    }
}