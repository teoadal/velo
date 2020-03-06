using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Velo.Settings;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Boos
{
    public class BooRepository : IBooRepository, IDisposable
    {
        public ISettings Settings { get; }

        public ISession Session { get; }

        public bool Disposed { get; private set; }
        
        private Dictionary<int, Boo> _storage;
        
        public BooRepository(ISettings settings, ISession session)
        {
            Settings = settings;
            Session = session;
            
            _storage = new Dictionary<int, Boo>();
        }

        public void AddElement(Boo element)
        {
            lock (_storage)
            {
                if (element.Id < 0) throw new InvalidOperationException($"Id '{element.Id}' less zero");
                _storage.Add(element.Id, element);    
            }
        }

        public Task AddElementAsync(Boo element)
        {
            AddElement(element);
            return Task.CompletedTask;
        }
        
        public bool Contains(int id)
        {
            return _storage.ContainsKey(id);
        }

        public Boo GetElement(int id)
        {
            return _storage[id];
        }

        public Task<Boo> GetElementAsync(int id)
        {
            return Task.FromResult(_storage[id]);
        }
        
        public void UpdateElement(int id, Action<Boo> update)
        {
            update(_storage[id]);
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}