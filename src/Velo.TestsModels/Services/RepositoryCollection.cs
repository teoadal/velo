namespace Velo.TestsModels.Services
{
    public class RepositoryCollection
    {
        public IRepository DataRepository { get; }
        
        public IRepository UserRepository { get; }

        public RepositoryCollection(IRepository dataRepository, IRepository userRepository)
        {
            DataRepository = dataRepository;
            UserRepository = userRepository;
        }
    }
}