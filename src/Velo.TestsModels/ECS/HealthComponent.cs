using Velo.ECS;

namespace Velo.TestsModels.ECS
{
    public class HealthComponent : IComponent
    {
        public int Value { get; set; }
        
        public void Dispose()
        {
        }
    }
}