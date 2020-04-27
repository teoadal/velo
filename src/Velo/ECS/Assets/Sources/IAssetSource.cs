using System;
using System.Collections.Generic;

namespace Velo.ECS.Assets.Sources
{
    public interface IAssetSource : IDisposable
    {
        IEnumerable<Asset> GetAssets();
    }
}