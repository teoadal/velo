using System.Collections.Generic;

namespace Velo.ECS.Assets.Sources
{
    internal sealed class MemoryAssetSource : IAssetSource
    {
        private readonly Asset[] _assets;

        public MemoryAssetSource(Asset[] assets)
        {
            _assets = assets;
        }

        public IEnumerable<Asset> GetAssets()
        {
            return _assets;
        }

        public void Dispose()
        {
        }
    }
}