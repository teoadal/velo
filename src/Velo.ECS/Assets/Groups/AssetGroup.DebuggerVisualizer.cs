using System.Diagnostics;
using System.Linq;

namespace Velo.ECS.Assets.Groups
{
    internal sealed class DebuggerVisualizer<TAsset>
        where TAsset : Asset
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public TAsset[] Items => _group.ToArray();

        private readonly AssetGroup<TAsset> _group;

        public DebuggerVisualizer(AssetGroup<TAsset> assetGroup)
        {
            _group = assetGroup;
        }
    }
}