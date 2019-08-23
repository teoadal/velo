using System;

using Velo.Mapping;

namespace Velo.TestsModels.Services
{
    public interface IDataService
    {
        IConfiguration Configuration { get; }

        IMapper<Foo> Mapper { get; }

        string Name { get; }

        IDataRepository Repository { get; }
    }

    public class DataService : IDataService
    {
        public IConfiguration Configuration { get; }

        public IMapper<Foo> Mapper { get; }

        public string Name { get; }

        public IDataRepository Repository { get; }

        public DataService(IConfiguration configuration, IMapper<Foo> mapper, IDataRepository repository)
        {
            Configuration = configuration;
            Mapper = mapper;
            Name = Guid.NewGuid().ToString("N");
            Repository = repository;
        }
    }
}