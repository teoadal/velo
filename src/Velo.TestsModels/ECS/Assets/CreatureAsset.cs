using Velo.ECS;
using Velo.ECS.Assets;

namespace Velo.TestsModels.ECS.Assets
{
    public class CreatureAsset : Asset
    {
        public ParametersComponent Parameters { get; }
        
        public CreatureAsset(int id, IComponent[] components) : base(id, components)
        {
            Parameters = GetComponent<ParametersComponent>();
        }
    }
}