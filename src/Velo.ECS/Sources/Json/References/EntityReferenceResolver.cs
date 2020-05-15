using System;
using System.IO;
using System.Reflection;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.References
{
    internal sealed class EntityReferenceResolver<TOwner, TEntity> : ReferenceResolver<TOwner, TEntity>
        where TOwner : class where TEntity : class, IEntity
    {
        private readonly Func<TOwner, TEntity?> _entityGetter;
        private readonly Action<TOwner, TEntity?> _entitySetter;

        private readonly string _propertyName;

        public EntityReferenceResolver(
            PropertyInfo property,
            SourceDescriptions descriptions,
            Func<int, TEntity> resolver)
            : base(descriptions, resolver)
        {
            _propertyName = property.Name;

            var owner = Typeof<TOwner>.Raw;
            _entityGetter = (Func<TOwner, TEntity?>) ExpressionUtils.BuildGetter(owner, property);
            _entitySetter = (Action<TOwner, TEntity?>) ExpressionUtils.BuildSetter(owner, property);
        }


        public override void Read(JsonObject source, TOwner instance)
        {
            var id = (JsonValue) source[_propertyName];
            var entity = ReadEntity(id);

            if (entity != null)
            {
                _entitySetter(instance, entity);
            }
        }

        public override void Serialize(TOwner instance, TextWriter output)
        {
            var entity = _entityGetter(instance);
            SerializeEntity(entity, output);
        }

        public override void Write(TOwner instance, JsonObject output)
        {
            var entity = _entityGetter(instance);
            output.Add(_propertyName, WriteEntity(entity));
        }
    }
}