using System;

using Velo.Mapping;

namespace Velo.TestsModels.Services
{
    public interface IBooService
    {
        IConfiguration Configuration { get; }

        IMapper<Boo> Mapper { get; }

        string Name { get; }
        
        IBooRepository Repository { get; }
    }

    public class BooService : IBooService
    {
        public IConfiguration Configuration { get; }

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
    }
}