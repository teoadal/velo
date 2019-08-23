namespace Velo.TestsModels.Services
{
    public interface IUserRepository
    {
        IConfiguration Configuration { get; }

        ISession Session { get; }
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