using System;
using Velo.Collections.Local;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Factory;
using Velo.ECS.Components;
using Velo.Extensions;
using Velo.Serialization;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;

namespace Velo.ECS.Sources.Json
{
    internal interface IJsonEntityReader<out TEntity>
        where TEntity : class, IEntity
    {
        TEntity Read(JsonObject entityData);
    }

    internal sealed class JsonEntityReader : IJsonEntityReader<Asset>, IJsonEntityReader<Actor>
    {
        private readonly IActorFactory _actorFactory;
        private readonly AssetFactory _assetFactory;
        private readonly IComponentFactory _componentFactory;
        private readonly IConvertersCollection _jsonConverters;
        private readonly SourceDescriptions _sourceDescriptions;

        public JsonEntityReader(
            IActorFactory actorFactory,
            AssetFactory assetFactory,
            IComponentFactory componentFactory,
            IConvertersCollection jsonConverters,
            SourceDescriptions sourceDescriptions)
        {
            _actorFactory = actorFactory;
            _assetFactory = assetFactory;
            _componentFactory = componentFactory;
            _jsonConverters = jsonConverters;
            _sourceDescriptions = sourceDescriptions;
        }

        private int ReadId(JsonObject entityData)
        {
            return _jsonConverters.Read<int>(entityData[nameof(IEntity.Id)]);
        }

        private IComponent[] ReadComponents(JsonObject entityData)
        {
            var componentsData = (JsonObject) entityData[nameof(IEntity.Components)];

            var components = new LocalList<IComponent>();

            foreach (var (componentName, data) in componentsData)
            {
                var componentType = _sourceDescriptions.GetComponentType(componentName);

                var component = _componentFactory.Create(componentType);

                var componentConverter = (IObjectConverter) _jsonConverters.Get(componentType);
                components.Add((IComponent) componentConverter.FillObject(data, component));
            }

            return components.ToArray();
        }

        private bool TryGetEntityType(JsonObject entityData, out Type entityType)
        {
            if (!entityData.TryGet("_type", out var typeData))
            {
                entityType = null!;
                return false;
            }

            var entityTypeName = _jsonConverters.Read<string>(typeData);
            entityType = _sourceDescriptions.GetEntityType(entityTypeName);

            return true;
        }

        Actor IJsonEntityReader<Actor>.Read(JsonObject actorData)
        {
            var id = ReadId(actorData);
            var components = ReadComponents(actorData);

            if (!TryGetEntityType(actorData, out var actorType))
            {
                return _actorFactory.Create(components, id);
            }

            var actor = _actorFactory.Create(actorType, components, id);
            var actorConverter = (IObjectConverter) _jsonConverters.Get(actorType);
            return (Actor) actorConverter.FillObject(actorData, actor);
        }

        Asset IJsonEntityReader<Asset>.Read(JsonObject assetData)
        {
            var id = ReadId(assetData);
            var components = ReadComponents(assetData);

            if (!TryGetEntityType(assetData, out var assetType))
            {
                return _assetFactory.Create(components, id);
            }

            var asset = _assetFactory.Create(assetType, components, id);
            var entityConverter = (IObjectConverter) _jsonConverters.Get(assetType);
            return (Asset) entityConverter.FillObject(assetData, asset);
        }
    }
}