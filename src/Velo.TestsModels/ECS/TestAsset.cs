using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json;

namespace Velo.TestsModels.ECS
{
    public class TestAsset: Asset
    {
        public int[] Array { get; set; }
        
        [SerializeReference]
        public Asset Reference { get; set; }
        
        public TestAsset(int id, IComponent[] components) : base(id, components)
        {
        }
    }
}