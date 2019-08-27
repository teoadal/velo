namespace Velo.TestsModels.Services
{
    public class CircularDependencyService
    {
        // ReSharper disable once UnusedParameter.Local
        public CircularDependencyService(CircularDependencyService service)
        {
        }
    }
}