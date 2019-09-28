using Velo.TestsModels.Domain;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Boos
{
    public interface IBooRepository : IRepository<Boo>
    {
    }

    public class BooRepository : IBooRepository
    {
        public IConfiguration Configuration { get; }

        public ISession Session { get; }

        public BooRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
            Session = session;
        }

        public Boo GetElement(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}