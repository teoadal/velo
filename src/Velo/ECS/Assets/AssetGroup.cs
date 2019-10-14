namespace Velo.ECS.Assets
{
    public sealed class AssetGroup<TAsset> : EntityGroup<TAsset>, IAssetGroup
        where TAsset : Asset
    {
        internal AssetGroup()
        {
        }

        void IAssetGroup.OnAddedToContext(Asset asset)
        {
            if (asset is TAsset found)
            {
                Add(found);
            }
        }

        void IAssetGroup.Initialize(AssetContext context)
        {
            foreach (var asset in context)
            {
                if (asset is TAsset found)
                {
                    Add(found);
                }
            }
        }
    }

    internal interface IAssetGroup
    {
        void Initialize(AssetContext context);

        void OnAddedToContext(Asset asset);
    }
}