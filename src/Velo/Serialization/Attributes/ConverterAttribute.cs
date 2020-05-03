using System;
using System.Reflection;
using Velo.Serialization.Converters;
using Velo.Utils;

namespace Velo.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class ConverterAttribute : Attribute
    {
        public static bool IsDefined(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(ConverterAttribute));
        }

        public readonly Type ConverterType;

        public ConverterAttribute(Type converterType)
        {
            if (converterType == null) throw Error.Null(nameof(converterType));
            if (!typeof(IJsonConverter).IsAssignableFrom(converterType))
            {
                throw Error.InvalidOperation(
                    $"Type {ReflectionUtils.GetName(converterType)} must implement {nameof(IJsonConverter)}");
            }

            ConverterType = converterType;
        }
    }
}