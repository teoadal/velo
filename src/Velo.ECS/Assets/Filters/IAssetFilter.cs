namespace Velo.ECS.Assets.Filters
{
    public interface IAssetFilter
    {
        int Length { get; }

        bool Contains(int assetId);
    }
}