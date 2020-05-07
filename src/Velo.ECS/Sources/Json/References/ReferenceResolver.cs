using System;
using System.IO;
using System.Reflection;
using Velo.Serialization;
using Velo.Serialization.Models;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.References
{
    internal abstract class ReferenceResolver<TOwner, TEntity>
        where TOwner : class where TEntity : class, IEntity
    {
        public static ReferenceResolver<TOwner, TEntity> Build(PropertyInfo property, Func<int, TEntity> resolver)
        {
            var propertyType = property.PropertyType;
            if (ReflectionUtils.IsListLikeGenericType(propertyType, out _))
            {
                return new ListReferenceResolver<TOwner, TEntity>(property, resolver);
            }

            if (ReflectionUtils.IsArrayLikeGenericType(propertyType, out _))
            {
                return new ArrayReferenceResolver<TOwner, TEntity>(property, resolver);
            }

            return new EntityReferenceResolver<TOwner, TEntity>(property, resolver);
        }

        private readonly Func<int, TEntity> _resolver;

        protected ReferenceResolver(Func<int, TEntity> resolver)
        {
            _resolver = resolver;
        }

        public abstract void Read(JsonObject source, TOwner instance);

        public abstract void Serialize(TOwner instance, TextWriter output);

        protected TEntity? ReadEntity(JsonData idData)
        {
            var idValue = (JsonValue) idData;

            if (idValue.Type == JsonDataType.Null) return null!;

            var id = idValue.Type == JsonDataType.String
                ? SourceDescriptions.GetOrAddAlias(idValue.Value)
                : int.Parse(idValue.Value);

            return _resolver(id);
        }

        protected void SerializeEntity(TEntity? entity, TextWriter output)
        {
            if (entity == null)
            {
                output.Write(JsonValue.NullToken);
                return;
            }

            var entityId = entity.Id;
            if (SourceDescriptions.TryGetAlias(entityId, out var alias))
            {
                output.WriteString(alias);
            }
            else
            {
                output.Write(entityId);
            }
        }
    }
}