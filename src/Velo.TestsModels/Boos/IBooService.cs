using System;
using Velo.Mapping;
using Velo.Settings;

namespace Velo.TestsModels.Boos
{
    public interface IBooService : IDisposable
    {
        ISettings Settings { get; }

        IMapper<Boo> Mapper { get; }

        string Name { get; }

        IBooRepository Repository { get; }
    }
}