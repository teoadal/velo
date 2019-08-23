namespace Velo.TestsModels.Services
{
    public interface IUserRepository : IRepository
    {
    }

    public class UserRepository : IUserRepository
    {
        public IConfiguration Configuration { get; }

        public ISession Session { get; }

        public UserRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
            Session = session;
        }
    }
}