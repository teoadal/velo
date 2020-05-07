using System;
using Velo.ECS.Assets.Factory;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json.Objects;

namespace Velo.ECS.Assets.Sources.Json
{
    internal sealed class AssetConverter<TAsset> : EntityConverter<TAsset>
        where TAsset : Asset
    {
        private readonly AssetFactory _assetFactory;

        public AssetConverter(AssetFactory assetFactory, IServiceProvider services)
            : base(services, typeof(TAsset) != typeof(Asset))
        {
            _assetFactory = assetFactory;
        }

        protected override TAsset CreateEntity(Type? assetType, int id, IComponent[]? components)
        {
            var asset = assetType == null
                ? _assetFactory.Create(id, components)
                : _assetFactory.Create(assetType, id, components);

            return (TAsset) asset;
        }
    }
}