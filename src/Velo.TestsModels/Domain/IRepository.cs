using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Domain
{
    public interface IRepository
    {
        IConfiguration Configuration { get; }

        ISession Session { get; }
    }

    public interface IRepository<TElement> : IRepository
    {
        void AddElement(TElement element);
        
        TElement GetElement(int id);
    }
}