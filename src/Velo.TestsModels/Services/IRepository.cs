namespace Velo.TestsModels.Services
{
    public interface IRepository
    {
        IConfiguration Configuration { get; }

        ISession Session { get; }
    }
    
    public interface IRepository<TElement> : IRepository
    {
        TElement GetElement(int id);
    }
}