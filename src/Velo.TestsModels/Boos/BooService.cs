using System;
using Velo.Mapping;
using Velo.Settings;

namespace Velo.TestsModels.Boos
{
    public class BooService : IBooService
    {
        public ISettings Settings { get; }

        public bool Disposed { get; private set; }
        
        public IMapper<Boo> Mapper { get; }

        public string Name { get; }

        public IBooRepository Repository { get; }

        public BooService(ISettings settings, IMapper<Boo> mapper, IBooRepository repository)
        {
            Settings = settings;
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