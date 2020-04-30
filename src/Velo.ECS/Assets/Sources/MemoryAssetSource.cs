using System.Collections.Generic;
using Velo.ECS.Sources;

namespace Velo.ECS.Assets.Sources
{
    internal sealed class MemoryAssetSource : MemorySource<Asset>, IAssetSource
    {
        public MemoryAssetSource(IEnumerable<Asset> assets)
            : base(assets)
        {
        }

        public IEnumerable<Asset> GetAssets() => this;
    }
}