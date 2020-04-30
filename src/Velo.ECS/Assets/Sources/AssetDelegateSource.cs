using System;
using System.Collections.Generic;

namespace Velo.ECS.Assets.Sources
{
    public class AssetDelegateSource : IAssetSource
    {
        private readonly Func<IAssetSourceContext, IEnumerable<Asset>> _builder;

        public AssetDelegateSource(Func<IAssetSourceContext, IEnumerable<Asset>> builder)
        {
            _builder = builder;
        }

        public IEnumerable<Asset> GetAssets(IAssetSourceContext sourceContext)
        {
            return _builder(sourceContext);
        }
    }
}