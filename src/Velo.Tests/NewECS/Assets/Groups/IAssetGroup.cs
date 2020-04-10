using System.Collections.Generic;

namespace Velo.Tests.NewECS.Assets.Groups
{
    public interface IAssetGroup
    {
        int Length { get; }

        bool Contains(int actorId);
    }

    public interface IAssetGroup<TAsset> : IAssetGroup, IEnumerable<TAsset>
        where TAsset : Asset
    {
        bool TryGet(int assetId, out TAsset asset);
    }
}