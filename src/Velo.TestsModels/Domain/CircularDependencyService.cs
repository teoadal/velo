namespace Velo.TestsModels.Domain
{
    public class CircularDependencyService
    {
        // ReSharper disable once UnusedParameter.Local
        public CircularDependencyService(CircularDependencyService service)
        {
        }
    }
}