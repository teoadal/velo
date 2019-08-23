using Velo.Mapping;

namespace Velo.TestsModels.Services
{
    public interface IDataService
    {
        IConfiguration Configuration { get; }

        IMapper<Foo> Mapper { get; }

        IDataRepository Repository { get; }
    }

    public class DataService : IDataService
    {
        public IConfiguration Configuration { get; }

        public IMapper<Foo> Mapper { get; }

        public IDataRepository Repository { get; }

        public DataService(IConfiguration configuration, IMapper<Foo> mapper, IDataRepository repository)
        {
            Configuration = configuration;
            Mapper = mapper;
            Repository = repository;
        }
    }
}