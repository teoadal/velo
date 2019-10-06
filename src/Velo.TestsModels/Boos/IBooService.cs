using Velo.Mapping;
using Velo.TestsModels.Infrastructure;

namespace Velo.TestsModels.Boos
{
    public interface IBooService
    {
        IConfiguration Configuration { get; }

        IMapper<Boo> Mapper { get; }

        string Name { get; }

        IBooRepository Repository { get; }
    }
}