using System;
using System.Collections.Generic;
using System.IO;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json.Properties;
using Velo.Extensions;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Serialization.Objects;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.Objects
{
    internal abstract class EntityConverter<TEntity> : IJsonConverter<TEntity>
        where TEntity : class, IEntity
    {
        private const string TypeProperty = "_type";

        public bool IsPrimitive => false;

        private readonly string? _customTypeName;
        private readonly Dictionary<string, IPropertyConverter<TEntity>> _properties;

        private readonly IdConverter _idConverter;
        private readonly ComponentsConverter _componentsConverter;

        protected EntityConverter(IServiceProvider services, bool isCustomType)
        {
            _properties = PropertyConverter<TEntity>.CreateCollection(services);

            _customTypeName = isCustomType
                ? SourceDescriptions.BuildTypeName(Typeof<TEntity>.Raw)
                : null;

            _idConverter = (IdConverter) _properties[nameof(IEntity.Id)];
            _componentsConverter = (ComponentsConverter) _properties[nameof(IEntity.Components)];
        }

        public TEntity Read(JsonData jsonData)
        {
            var entityData = (JsonObject) jsonData;

            var id = (int) _idConverter.ReadValue(entityData)!;
            var components = (IComponent[]?) _componentsConverter.ReadValue(entityData);

            if (_customTypeName == null)
            {
                return CreateEntity(null, id, components);
            }

            var entity = CreateEntity(Typeof<TEntity>.Raw, id, components);

            foreach (var (propertyName, propertyConverter) in _properties)
            {
                if (propertyName == nameof(IEntity.Id) || propertyName == nameof(IEntity.Components)) continue;

                propertyConverter.Read(entityData, entity);
            }

            return entity;
        }

        public void Serialize(TEntity instance, TextWriter output)
        {
            output.Write('{');

            var first = true;
            foreach (var (name, converter) in _properties)
            {
                if (first) first = false;
                else output.Write(',');

                output.WriteProperty(name);
                converter.Serialize(instance, output);
            }

            if (_customTypeName != null)
            {
                output.Write(",\"");
                output.Write(TypeProperty);
                output.Write("\": \"");
                output.Write(_customTypeName);
                output.Write('\"');
            }

            output.Write('}');
        }

        protected abstract TEntity CreateEntity(Type? entityType, int id, IComponent[]? components);

        object IJsonConverter.ReadObject(JsonData jsonData) => Read(jsonData)!;

        void IJsonConverter.SerializeObject(object value, TextWriter writer) => Serialize((TEntity) value, writer);

        TEntity IJsonConverter<TEntity>.Deserialize(JsonTokenizer _) => throw Error.NotSupported();
        object IJsonConverter.DeserializeObject(JsonTokenizer _) => throw Error.NotSupported();
        JsonData IJsonConverter.WriteObject(object value) => throw Error.NotSupported();
        JsonData IJsonConverter<TEntity>.Write(TEntity value) => throw Error.NotSupported();
    }
}