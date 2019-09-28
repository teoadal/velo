using System;
using Velo.Mapping;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Foos
{
    public interface IFooService
    {
        Guid Id { get; }
        
        IConfiguration Configuration { get; }

        bool Disposed { get; }
        
        IMapper<Foo> Mapper { get; }

        string Name { get; }

        IFooRepository Repository { get; }
    }

    public class FooService : IFooService, IDisposable
    {
        public Guid Id { get; }
        
        public IConfiguration Configuration { get; }

        public bool Disposed { get; set; }
        
        public IMapper<Foo> Mapper { get; }

        public string Name { get; }

        public IFooRepository Repository { get; }

        public FooService(IConfiguration configuration, IMapper<Foo> mapper, IFooRepository repository)
        {
            Id = Guid.NewGuid();
            Configuration = configuration;
            Mapper = mapper;
            Name = Guid.NewGuid().ToString("N");
            Repository = repository;
        }

        public void Dispose()
        {
            Disposed = true;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}