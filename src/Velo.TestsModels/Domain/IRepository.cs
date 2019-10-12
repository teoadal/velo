using System;
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

        bool Contains(int id);
        
        TElement GetElement(int id);

        void UpdateElement(int id, Action<TElement> update);
    }
}