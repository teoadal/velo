using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.References
{
    internal sealed class ArrayReferenceResolver<TOwner, TEntity> : ReferenceResolver<TOwner, TEntity>
        where TOwner : class where TEntity : class, IEntity
    {
        private readonly Func<TOwner, IEnumerable<TEntity>> _entityGetter;
        private readonly Action<TOwner, TEntity?[]> _entitySetter;

        private readonly string _propertyName;

        public ArrayReferenceResolver(
            PropertyInfo property,
            SourceDescriptions descriptions,
            Func<int, TEntity> resolver)
            : base(descriptions, resolver)
        {
            _propertyName = property.Name;

            var owner = Typeof<TOwner>.Raw;
            _entityGetter = (Func<TOwner, IEnumerable<TEntity>>) ExpressionUtils.BuildGetter(owner, property);
            _entitySetter = (Action<TOwner, TEntity?[]>) ExpressionUtils.BuildSetter(owner, property);
        }

        public override void Read(JsonObject source, TOwner instance)
        {
            var ids = (JsonArray) source[_propertyName];

            var entities = new TEntity?[ids.Length];

            for (var i = entities.Length - 1; i >= 0; i--)
            {
                entities[i] = ReadEntity(ids[i]);
            }

            _entitySetter(instance, entities);
        }

        public override void Serialize(TOwner instance, TextWriter output)
        {
            var entities = _entityGetter(instance);

            output.Write('[');

            var first = true;
            foreach (var entity in entities)
            {
                if (first) first = false;
                else output.Write(',');

                SerializeEntity(entity, output);
            }

            output.Write(']');
        }

        public override void Write(TOwner instance, JsonObject output)
        {
            var entities = _entityGetter(instance);
            var ids = new List<JsonData>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var entity in entities)
            {
                ids.Add(WriteEntity(entity));
            }

            output.Add(_propertyName, new JsonArray(ids.ToArray()));
        }
    }
}