using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Velo.Collections;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Assets.Groups;
using Velo.ECS.Assets.Sources;
using Velo.ECS.Components;
using Velo.Threading;
using Velo.Utils;

namespace Velo.ECS.Assets.Context
{
    [DebuggerTypeProxy(typeof(AssetContextDebugVisualizer))]
    [DebuggerDisplay("Length = {" + nameof(Length) + "}")]
    internal sealed partial class AssetContext : IAssetContext
    {
        public int Length => _assets.Length;

        private readonly Asset[] _assets;
        private readonly Dictionary<int, IAssetFilter> _filters;
        private readonly Dictionary<int, IAssetGroup> _groups;
        private readonly Dictionary<int, object> _singleAssets;

        public AssetContext(AssetSourceContext sourceContext)
            : this(sourceContext.GetAssets())
        {
        }

        public AssetContext(IEnumerable<Asset> assets)
        {
            _assets = assets.ToArray();

            CollectionUtils.EnsureUnique(
                _assets,
                asset => throw Error.AlreadyExists($"Asset with id '{asset.Id}' already exists"));

            _filters = new Dictionary<int, IAssetFilter>(32);
            _groups = new Dictionary<int, IAssetGroup>(32);
            _singleAssets = new Dictionary<int, object>();
        }

        public void AddFilter<TComponent>(IAssetFilter<TComponent> assetFilter) where TComponent : IComponent
        {
            var filterId = Typeof<IAssetFilter<TComponent>>.Id;
            _filters.Add(filterId, assetFilter);
        }

        public void AddFilter<TComponent1, TComponent2>(IAssetFilter<TComponent1, TComponent2> assetFilter)
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            var filterId = Typeof<IAssetFilter<TComponent1, TComponent2>>.Id;
            _filters.Add(filterId, assetFilter);
        }

        public void AddGroup<TAsset>(IAssetGroup<TAsset> assetGroup) where TAsset : Asset
        {
            var groupId = Typeof<TAsset>.Id;
            _groups.Add(groupId, assetGroup);
        }

        public bool Contains(int assetId)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var asset in _assets)
            {
                if (asset.Id == assetId) return true;
            }

            return false;
        }

        public Asset Get(int assetId)
        {
            return TryGet(assetId, out var asset)
                ? asset
                : throw Error.NotFound($"Asset with id '{assetId}' not found in current context");
        }

        public IAssetFilter<TComponent> GetFilter<TComponent>() where TComponent : IComponent
        {
            var filterId = Typeof<IAssetFilter<TComponent>>.Id;

            IAssetFilter filter;
            using (Lock.Enter(_filters))
            {
                // ReSharper disable once InvertIf
                if (!_filters.TryGetValue(filterId, out filter))
                {
                    filter = new AssetFilter<TComponent>(_assets);
                    _filters.Add(filterId, filter);
                }
            }

            return (IAssetFilter<TComponent>) filter;
        }

        public IAssetFilter<TComponent1, TComponent2> GetFilter<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            var filterId = Typeof<IAssetFilter<TComponent1, TComponent2>>.Id;

            IAssetFilter filter;
            using (Lock.Enter(_filters))
            {
                // ReSharper disable once InvertIf
                if (!_filters.TryGetValue(filterId, out filter))
                {
                    filter = new AssetFilter<TComponent1, TComponent2>(_assets);
                    _filters.Add(filterId, filter);
                }
            }

            return (IAssetFilter<TComponent1, TComponent2>) filter;
        }

        public IAssetGroup<TAsset> GetGroup<TAsset>() where TAsset : Asset
        {
            var actorGroupId = Typeof<TAsset>.Id;

            IAssetGroup assetGroup;
            using (Lock.Enter(_groups))
            {
                // ReSharper disable once InvertIf
                if (!_groups.TryGetValue(actorGroupId, out assetGroup))
                {
                    assetGroup = new AssetGroup<TAsset>(_assets);
                    _groups.Add(actorGroupId, assetGroup);
                }
            }

            return (IAssetGroup<TAsset>) assetGroup;
        }

        public SingleAsset<TAsset> GetSingle<TAsset>() where TAsset : Asset
        {
            var typeId = Typeof<TAsset>.Id;

            object singleAsset;
            using (Lock.Enter(_singleAssets))
            {
                // ReSharper disable once InvertIf
                if (!_singleAssets.TryGetValue(typeId, out singleAsset))
                {
                    singleAsset = new SingleAsset<TAsset>(_assets);
                    _singleAssets.Add(typeId, singleAsset);
                }
            }

            return (SingleAsset<TAsset>) singleAsset;
        }

        public bool TryGet(int assetId, out Asset asset)
        {
            foreach (var exists in _assets)
            {
                if (exists.Id != assetId) continue;

                asset = exists;
                return true;
            }

            asset = null!;
            return false;
        }

        public IEnumerator<Asset> GetEnumerator()
        {
            return new ArrayEnumerator<Asset>(_assets);
        }

        public IEnumerable<Asset> Where<TArg>(Func<Asset, TArg, bool> filter, TArg arg)
        {
            return new ArrayWhereEnumerator<Asset, TArg>(_assets, filter, arg);
        }

        public void Dispose()
        {
            CollectionUtils.DisposeValuesIfDisposable(_filters);
            CollectionUtils.DisposeValuesIfDisposable(_groups);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}