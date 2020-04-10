using System;
using Velo.Mapping;
using Velo.Settings.Provider;

namespace Velo.TestsModels.Boos
{
    public interface IBooService : IDisposable
    {
        ISettingsProvider Settings { get; }

        IMapper<Boo> Mapper { get; }

        string Name { get; }

        IBooRepository Repository { get; }
    }
}