using System;
using Velo.Mapping;
using Velo.Settings.Provider;

namespace Velo.TestsModels.Foos
{
    public interface IFooService
    {
        Guid Id { get; }
        
        ISettingsProvider Settings { get; }

        bool Disposed { get; }
        
        IMapper<Foo> Mapper { get; }

        string Name { get; }

        IFooRepository Repository { get; }
    }
}