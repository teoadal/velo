using Velo.ECS;

namespace Velo.TestsModels.ECS
{
    public class ManaCostComponent : IComponent
    {
        public int Value { get; set; }
        
        public void Dispose()
        {
        }
    }
}