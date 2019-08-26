namespace Velo.TestsModels.Services
{
    public class RepositoryCollection
    {
        public IRepository BooRepository { get; }

        public IRepository FooRepository { get; }

        public RepositoryCollection(IRepository booRepository, IRepository fooRepository)
        {
            BooRepository = booRepository;
            FooRepository = fooRepository;
        }
    }
}