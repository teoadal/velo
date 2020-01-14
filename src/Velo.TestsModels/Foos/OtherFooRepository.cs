using System;
using Velo.Ordering;
using Velo.Settings;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Foos
{
    [Order(2)]
    public class OtherFooRepository : IFooRepository
    {
        public IConfiguration Configuration { get; }

        public ISession Session { get; }

        public OtherFooRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
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