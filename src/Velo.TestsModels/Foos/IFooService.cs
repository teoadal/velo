using System;
using Velo.Mapping;
using Velo.Settings;

namespace Velo.TestsModels.Foos
{
    public interface IFooService
    {
        Guid Id { get; }
        
        ISettings Settings { get; }

        bool Disposed { get; }
        
        IMapper<Foo> Mapper { get; }

        string Name { get; }

        IFooRepository Repository { get; }
    }
}