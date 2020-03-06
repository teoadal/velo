using System;
using Velo.Mapping;
using Velo.Settings;

namespace Velo.TestsModels.Foos
{
    public class FooService : IFooService, IDisposable
    {
        public Guid Id { get; }
        
        public ISettings Settings { get; }

        public bool Disposed { get; private set; }
        
        public IMapper<Foo> Mapper { get; }

        public string Name { get; }

        public IFooRepository Repository { get; }

        public FooService(ISettings settings, IMapper<Foo> mapper, IFooRepository repository)
        {
            Id = Guid.NewGuid();
            Settings = settings;
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