using System;
using System.IO;
using System.Reflection;
using Velo.DependencyInjection;
using Velo.ECS.Assets.Context;
using Velo.ECS.Sources.Context;
using Velo.ECS.Sources.Json.References;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Assets.Sources.Json
{
    internal sealed class AssetReferencesConverter<TOwner, TAsset> : IPropertyConverter<TOwner>
        where TOwner : class where TAsset : Asset
    {
        private readonly Lazy<IAssetContext> _assetContext;
        private readonly IEntitySourceContext<Asset> _sourceContext;

        private readonly ReferenceResolver<TOwner, TAsset> _resolver;

        public AssetReferencesConverter(
            DependencyProvider serviceProvider,
            IEntitySourceContext<Asset> sourceContext,
            PropertyInfo property)
        {
            _assetContext = new Lazy<IAssetContext>(serviceProvider.GetRequiredService<IAssetContext>);
            _resolver = ReferenceResolver<TOwner, TAsset>.Build(property, GetEntity);
            _sourceContext = sourceContext;
        }

        public void Read(JsonObject source, TOwner instance)
        {
            _resolver.Read(source, instance);
        }

        public void Serialize(TOwner instance, TextWriter output)
        {
            _resolver.Serialize(instance, output);
        }

        private TAsset GetEntity(int id)
        {
            var asset = _sourceContext.IsStarted
                ? _sourceContext.Get(id)
                : _assetContext.Value.Get(id);

            return (TAsset) asset;
        }

        void IPropertyConverter<TOwner>.Deserialize(JsonTokenizer _, TOwner instance) => throw Error.NotSupported();
        void IPropertyConverter<TOwner>.Write(TOwner instance, JsonObject _) => throw Error.NotSupported();
    }
}