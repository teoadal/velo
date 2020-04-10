using System;
using Velo.Settings.Provider;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Domain
{
    public interface IRepository
    {
        ISettingsProvider Settings { get; }

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