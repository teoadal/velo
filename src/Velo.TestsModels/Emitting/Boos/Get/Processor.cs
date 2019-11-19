using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;

namespace Velo.TestsModels.Emitting.Boos.Get
{
    public sealed class Processor : IQueryProcessor<Query, Boo>
    {
        private readonly IBooRepository _repository;

        public Processor(IBooRepository repository)
        {
            _repository = repository;
        }
        
        public Task<Boo> Process(Query request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_repository.GetElement(request.Id));
        }
    }
}