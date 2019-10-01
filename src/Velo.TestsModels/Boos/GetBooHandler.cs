using Velo.CQRS.Queries;

namespace Velo.TestsModels.Boos
{
    public sealed class GetBooHandler : IQueryHandler<GetBoo, Boo>
    {
        private readonly IBooRepository _repository;

        public GetBooHandler(IBooRepository repository)
        {
            _repository = repository;
        }

        public Boo Execute(GetBoo query)
        {
            return _repository.GetElement(query.Id);
        }
    }
}