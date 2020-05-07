using System.Collections.Generic;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json.References;

namespace Velo.TestsModels.ECS
{
    public class TestComponent3 : IComponent
    {
        [SerializeReference] 
        public TestAsset Asset { get; set; }

        [SerializeReference] 
        public Asset[] AssetsArray { get; set; }

        [SerializeReference] 
        public List<Asset> AssetsList { get; set; }
    }
}