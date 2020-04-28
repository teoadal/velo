using System.Collections.Generic;

namespace Velo.ECS.Assets.Sources
{
    public interface IAssetSource
    {
        IEnumerable<Asset> GetAssets();
    }
}