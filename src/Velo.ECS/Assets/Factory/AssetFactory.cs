using System;
using Velo.Collections;
using Velo.ECS.Components;
using Velo.Utils;

namespace Velo.ECS.Assets.Factory
{
    internal sealed class AssetFactory : DangerousVector<int, IAssetBuilder>
    {
        private readonly IAssetBuilder[] _builders;
        private readonly Func<int, Type, IAssetBuilder> _findOrCreate;

        public AssetFactory(IAssetBuilder[]? builders = null)
        {
            _builders = builders ?? Array.Empty<IAssetBuilder>();
            _findOrCreate = FindOrCreateBuilder;
        }

        public Asset Create(int id, IComponent[]? components = null)
        {
            return new Asset(id, components ?? Array.Empty<IComponent>());
        }

        public Asset Create(Type entityType, int id, IComponent[]? components = null)
        {
            var builder = GetOrAdd(Typeof.GetTypeId(entityType), _findOrCreate, entityType);
            return builder.BuildAsset(id, components ?? Array.Empty<IComponent>());
        }

        private IAssetBuilder FindOrCreateBuilder(int _, Type assetType)
        {
            var builderType = typeof(IAssetBuilder<>).MakeGenericType(assetType);

            foreach (var builder in _builders)
            {
                if (builderType.IsInstanceOfType(builder))
                {
                    return builder;
                }
            }

            var instanceType = typeof(DefaultAssetBuilder<>).MakeGenericType(assetType);
            var instance = Activator.CreateInstance(instanceType);

            return (IAssetBuilder) instance;
        }
    }
}