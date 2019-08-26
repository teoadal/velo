using System;
using Velo.Mapping;

namespace Velo.TestsModels.Services
{
    public interface IFooService
    {
        IConfiguration Configuration { get; }

        IMapper<Foo> Mapper { get; }

        string Name { get; }

        IFooRepository Repository { get; }
    }

    public class FooService : IFooService
    {
        public IConfiguration Configuration { get; }

        public IMapper<Foo> Mapper { get; }

        public string Name { get; }

        public IFooRepository Repository { get; }

        public FooService(IConfiguration configuration, IMapper<Foo> mapper, IFooRepository repository)
        {
            Configuration = configuration;
            Mapper = mapper;
            Name = Guid.NewGuid().ToString("N");
            Repository = repository;
        }
    }
}