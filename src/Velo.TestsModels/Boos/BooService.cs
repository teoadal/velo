using System;
using Velo.Mapping;
using Velo.Settings.Provider;

namespace Velo.TestsModels.Boos
{
    public class BooService : IBooService
    {
        public ISettingsProvider Settings { get; }

        public bool Disposed { get; private set; }
        
        public IMapper<Boo> Mapper { get; }

        public string Name { get; }

        public IBooRepository Repository { get; }

        public BooService(ISettingsProvider settings, IMapper<Boo> mapper, IBooRepository repository)
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