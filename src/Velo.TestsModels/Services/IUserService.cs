using System;

using Velo.Mapping;

namespace Velo.TestsModels.Services
{
    public interface IUserService
    {
        IConfiguration Configuration { get; }

        IMapper<Boo> Mapper { get; }

        string Name { get; }
        
        IUserRepository Repository { get; }
    }

    public class UserService : IUserService
    {
        public IConfiguration Configuration { get; }

        public IMapper<Boo> Mapper { get; }

        public string Name { get; }
        
        public IUserRepository Repository { get; }

        public UserService(IConfiguration configuration, IMapper<Boo> mapper, IUserRepository repository)
        {
            Configuration = configuration;
            Mapper = mapper;
            Name = Guid.NewGuid().ToString("N");
            Repository = repository;
        }
    }
}