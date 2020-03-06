using System;
using Velo.Ordering;
using Velo.Settings;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Foos
{
    [Order(2)]
    public class OtherFooRepository : IFooRepository
    {
        public ISettings Settings { get; }

        public ISession Session { get; }

        public OtherFooRepository(ISettings settings, ISession session)
        {
            Settings = settings;
            Session = session;
        }

        public void AddElement(Foo element)
        {
            throw new NotImplementedException();
        }

        public bool Contains(int id)
        {
            throw new NotImplementedException();
        }

        public Foo GetElement(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateElement(int id, Action<Foo> update)
        {
            throw new NotImplementedException();
        }
    }
}