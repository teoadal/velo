using System;
using System.Collections.Generic;
using Velo.Ordering;
using Velo.Settings;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Foos
{
    [Order(1)]
    public class FooRepository : IFooRepository
    {
        public ISettings Settings { get; }

        public ISession Session { get; }

        private List<Foo> _foos;
        
        public FooRepository(ISettings settings, ISession session)
        {
            Settings = settings;
            Session = session;
            
            _foos = new List<Foo>();
        }

        public void AddElement(Foo element)
        {
            _foos.Add(element);
        }

        public bool Contains(int id)
        {
            return _foos.Exists(f => f.Int == id);
        }

        public Foo GetElement(int id)
        {
            return _foos.Find(f => f.Int == id);
        }

        public void UpdateElement(int id, Action<Foo> update)
        {
            throw new NotImplementedException();
        }
    }
}