using System;
using System.Collections;
using System.Reflection;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Sources.Json;
using Velo.Serialization.Attributes;
using Velo.Utils;

namespace Velo.ECS.Sources.Json.References
{
    /// <summary>
    /// Serialize <see cref="IEntity"/> as <see cref="IEntity.Id"/> (number)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SerializeReferenceAttribute : ConverterAttribute
    {
        public override Type GetConverterType(Type owner, PropertyInfo? property = null)
        {
            if (property == null) throw Error.InvalidOperation("Wrong way to use attribute");

            var propertyType = property.PropertyType;
            var entityType = typeof(ICollection).IsAssignableFrom(propertyType)
                ? ReflectionUtils.GetCollectionElementType(propertyType)
                : propertyType;

            return typeof(Asset).IsAssignableFrom(entityType)
                ? typeof(AssetReferencesConverter<,>).MakeGenericType(owner, entityType)
                : throw new NotImplementedException();
        }
    }
}