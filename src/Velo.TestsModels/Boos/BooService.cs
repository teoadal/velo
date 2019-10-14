using System;
using Velo.Mapping;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Boos
{
    public class BooService : IBooService
    {
        public IConfiguration Configuration { get; }

        public bool Disposed { get; private set; }
        
        public IMapper<Boo> Mapper { get; }

        public string Name { get; }

        public IBooRepository Repository { get; }

        public BooService(IConfiguration configuration, IMapper<Boo> mapper, IBooRepository repository)
        {
            Configuration = configuration;
            Mapper = mapper;
            Name = Guid.NewGuid().ToString("N");
            Repository = repository;
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}