namespace Velo.TestsModels.Services
{
    public interface IDataRepository : IRepository
    {
    }

    public class DataRepository : IDataRepository
    {
        public IConfiguration Configuration { get; }
        
        public ISession Session { get; }

        public DataRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
            Session = session;
        }
    }
}