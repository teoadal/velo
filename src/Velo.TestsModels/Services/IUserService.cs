using Velo.Mapping;

namespace Velo.TestsModels.Services
{
    public interface IUserService
    {
        IConfiguration Configuration { get; }

        IMapper<Boo> Mapper { get; }

        IUserRepository Repository { get; }
    }

    public class UserService : IUserService
    {
        public IConfiguration Configuration { get; }

        public IMapper<Boo> Mapper { get; }

        public IUserRepository Repository { get; }

        public UserService(IConfiguration configuration, IMapper<Boo> mapper, IUserRepository repository)
        {
            Configuration = configuration;
            Mapper = mapper;
            Repository = repository;
        }
    }
}