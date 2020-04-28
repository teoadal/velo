using System;
using System.Collections.Generic;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Assets.Groups;
using Velo.ECS.Components;

namespace Velo.ECS.Assets.Context
{
    public interface IAssetContext : IEnumerable<Asset>, IDisposable
    {
        void AddFilter<TComponent>(IAssetFilter<TComponent> assetFilter) where TComponent : IComponent;

        void AddFilter<TComponent1, TComponent2>(IAssetFilter<TComponent1, TComponent2> assetFilter)
            where TComponent1 : IComponent where TComponent2 : IComponent;

        void AddGroup<TAsset>(IAssetGroup<TAsset> assetGroup) where TAsset : Asset;

        bool Contains(int assetId);

        Asset Get(int assetId);
        
        IAssetFilter<TComponent> GetFilter<TComponent>() where TComponent : IComponent;

        IAssetFilter<TComponent1, TComponent2> GetFilter<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent;

        IAssetGroup<TAsset> GetGroup<TAsset>() where TAsset : Asset;

        SingleAsset<TAsset> GetSingle<TAsset>() where TAsset : Asset;

        bool TryGet(int assetId, out Asset asset);
        
        IEnumerable<Asset> Where<TArg>(Func<Asset, TArg, bool> filter, TArg arg);
    }
}