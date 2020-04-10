namespace Velo.Tests.NewECS.Assets.Filters
{
    public interface IAssetFilter
    {
        int Length { get; }

        bool Contains(int assetId);
    }
}