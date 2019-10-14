using System.Collections.Generic;
using Velo.Utils;

namespace Velo.ECS.Assets
{
    public sealed class AssetContext : EntityContext<Asset>
    {
        private readonly Dictionary<int, AssetFilter> _filters;
        private readonly Dictionary<int, IAssetGroup> _groups;

        public AssetContext()
        {
            _filters = new Dictionary<int, AssetFilter>();
            _groups = new Dictionary<int, IAssetGroup>();
        }

        public AssetFilter<TComponent1> GetFilter<TComponent1>() where TComponent1 : IComponent
        {
            var filterId = Typeof<AssetFilter<TComponent1>>.Id;

            if (!_filters.TryGetValue(filterId, out var filter))
            {
                filter = new AssetFilter<TComponent1>();
                filter.Initialize(this);
                
                _filters.Add(filterId, filter);
            }

            return (AssetFilter<TComponent1>) filter;
        }

        public AssetFilter<TComponent1, TComponent2> GetFilter<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            var filterId = Typeof<AssetFilter<TComponent1>>.Id;

            if (!_filters.TryGetValue(filterId, out var filter))
            {
                filter = new AssetFilter<TComponent1, TComponent2>();
                filter.Initialize(this);
                
                _filters.Add(filterId, filter);
            }

            return (AssetFilter<TComponent1, TComponent2>) filter;
        }

        public AssetGroup<TAsset> GetGroup<TAsset>() where TAsset: Asset
        {
            var groupId = Typeof<TAsset>.Id;

            if (!_groups.TryGetValue(groupId, out var assetGroup))
            {
                assetGroup = new AssetGroup<TAsset>();
                assetGroup.Initialize(this);
                
                _groups.Add(groupId, assetGroup);
            }

            return (AssetGroup<TAsset>) assetGroup;
        }
        
        protected override void OnAdded(Asset asset)
        {
            foreach (var filter in _filters.Values)
            {
                filter.OnAddedToContext(asset);
            }

            foreach (var assetGroup in _groups.Values)
            {
                assetGroup.OnAddedToContext(asset);
            }
        }
    }
}