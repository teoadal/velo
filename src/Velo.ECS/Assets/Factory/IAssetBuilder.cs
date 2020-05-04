using Velo.ECS.Components;
using Velo.ECS.Factory;

namespace Velo.ECS.Assets.Factory
{
    public interface IAssetBuilder
    {
        Asset BuildAsset(int assetId, IComponent[] components);
    }

    public interface IAssetBuilder<out TAsset> : IAssetBuilder
        where TAsset : Asset
    {
        TAsset Build(int assetId, IComponent[] components);
    }

    internal sealed class DefaultAssetBuilder<TAsset> : DefaultEntityBuilder<TAsset>, IAssetBuilder<TAsset>
        where TAsset : Asset
    {
        public Asset BuildAsset(int assetId, IComponent[] components)
        {
            return Build(assetId, components);
        }
    }
}