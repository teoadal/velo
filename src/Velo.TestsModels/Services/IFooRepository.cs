namespace Velo.TestsModels.Services
{
    public interface IFooRepository : IRepository<Foo>
    {
    }

    public class FooRepository : IFooRepository
    {
        public IConfiguration Configuration { get; }

        public ISession Session { get; }

        public FooRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
            Session = session;
        }

        public Foo GetElement(int id)
        {
            throw new System.NotImplementedException();
        }
    }

    public class OtherFooRepository : IFooRepository
    {
        public IConfiguration Configuration { get; }

        public ISession Session { get; }

        public OtherFooRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
            Session = session;
        }

        public Foo GetElement(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}