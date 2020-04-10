using System.Collections;
using System.Collections.Generic;

namespace Velo.Tests.NewECS.Assets.Groups
{
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

            asset = default;
            return false;
        }

        public IEnumerator<TAsset> GetEnumerator()
        {
            return (IEnumerator<TAsset>) _assets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _assets.GetEnumerator();
    }
}