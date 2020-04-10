using System;
using System.Collections.Generic;
using Velo.Ordering;
using Velo.Settings.Provider;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Foos
{
    [Order(1)]
    public class FooRepository : IFooRepository
    {
        public ISettingsProvider Settings { get; }

        public ISession Session { get; }

        private List<Foo> _foos;
        
        public FooRepository(ISettingsProvider settings, ISession session)
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