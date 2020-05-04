using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Velo.Collections.Enumerators;

namespace Velo.ECS.Assets.Groups
{
    [DebuggerTypeProxy(typeof(DebuggerVisualizer<>))]
    [DebuggerDisplay("Length = {" + nameof(Length) + "}")]
    internal sealed class AssetGroup<TAsset> : IAssetGroup<TAsset>
        where TAsset : Asset
    {
        public int Length => _assets.Length;

        private readonly TAsset[] _assets;

        public AssetGroup(Asset[] assets)
        {
            var assetList = new List<TAsset>(100);

            foreach (var asset in assets)
            {
                if (asset is TAsset found)
                {
                    assetList.Add(found);
                }
            }

            _assets = assetList.ToArray();
        }

        public bool Contains(int actorId) => TryGet(actorId, out _);

        public bool TryGet(int assetId, out TAsset asset)
        {
            foreach (var exists in _assets)
            {
                if (exists.Id != assetId) continue;

                asset = exists;
                return true;
            }

            asset = default!;
            return false;
        }

        public IEnumerable<TAsset> Where<TArg>(Func<TAsset, TArg, bool> filter, TArg arg)
        {
            return new ArrayWhereEnumerator<TAsset, TArg>(_assets, filter, arg);
        }

        public IEnumerator<TAsset> GetEnumerator()
        {
            return new ArrayEnumerator<TAsset>(_assets);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}