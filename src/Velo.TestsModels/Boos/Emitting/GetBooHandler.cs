using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Queries;

namespace Velo.TestsModels.Boos.Emitting
{
    public sealed class GetBooHandler : IQueryHandler<GetBoo, Boo>
    {
        private readonly IBooRepository _repository;

        public GetBooHandler(IBooRepository repository)
        {
            _repository = repository;
        }

        public Task<Boo> ExecuteAsync(GetBoo query, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetElement(query.Id));
        }
    }
}