using System.Collections.Generic;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Boos
{
    public interface IBooRepository : IRepository<Boo>
    {
    }

    public class BooRepository : IBooRepository
    {
        public IConfiguration Configuration { get; }

        public ISession Session { get; }

        private Dictionary<int, Boo> _storage;
        
        public BooRepository(IConfiguration configuration, ISession session)
        {
            Configuration = configuration;
            Session = session;
            
            _storage = new Dictionary<int, Boo>();
        }

        public void AddElement(Boo element)
        {
            _storage.Add(element.Id, element);
        }
        
        public Boo GetElement(int id)
        {
            return _storage[id];
        }
    }
}