using Velo.Serialization;

namespace Velo.TestsModels.Services
{
    public class CircularDependencyService
    {
        public CircularDependencyService(CircularDependencyService service)
        {
        }
    }
}