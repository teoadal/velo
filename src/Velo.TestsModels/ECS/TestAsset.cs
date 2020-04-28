using Velo.ECS.Assets;
using Velo.ECS.Components;

namespace Velo.TestsModels.ECS
{
    public class TestAsset: Asset
    {
        public int[] Array { get; set; }
        
        public TestAsset(int id, IComponent[] components) : base(id, components)
        {
        }
    }
}