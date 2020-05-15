using System.IO;
using System.Reflection;
using Velo.ECS.Assets.Context;
using Velo.ECS.Sources;
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
        private readonly IReference<IAssetContext> _assetContext;
        private readonly IEntitySourceContext<Asset> _sources;

        private readonly ReferenceResolver<TOwner, TAsset> _resolver;

        public AssetReferencesConverter(
            IReference<IAssetContext> assetContext,
            SourceDescriptions descriptions,
            IEntitySourceContext<Asset> sources,
            PropertyInfo property)
        {
            _assetContext = assetContext;
            _resolver = ReferenceResolver<TOwner, TAsset>.Build(property, descriptions, GetEntity);
            _sources = sources;
        }

        public void Read(JsonObject source, TOwner instance)
        {
            _resolver.Read(source, instance);
        }

        public void Serialize(TOwner instance, TextWriter output)
        {
            _resolver.Serialize(instance, output);
        }

        public void Write(TOwner instance, JsonObject output)
        {
            _resolver.Write(instance, output);
        }

        private TAsset GetEntity(int id)
        {
            var asset = _sources.IsStarted
                ? _sources.Get(id)
                : _assetContext.Value.Get(id);

            return (TAsset) asset;
        }

        void IPropertyConverter<TOwner>.Deserialize(JsonTokenizer _, TOwner instance) => throw Error.NotSupported();
    }
}