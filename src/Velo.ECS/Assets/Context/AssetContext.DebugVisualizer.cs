using System.Linq;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Assets.Groups;

namespace Velo.ECS.Assets.Context
{
    internal sealed partial class AssetContext
    {
        private sealed class AssetContextDebugVisualizer
        {
            // ReSharper disable UnusedMember.Local

            public Asset[] Assets => _context._assets;

            public IAssetFilter[] Filters => _context._filters.Values.ToArray();

            public IAssetGroup[] Groups => _context._groups.Values.ToArray();

            public object[] Singles => _context._singleAssets.Values.ToArray();

            // ReSharper restore UnusedMember.Local

            private readonly AssetContext _context;

            public AssetContextDebugVisualizer(AssetContext context)
            {
                _context = context;
            }
        }
    }
}