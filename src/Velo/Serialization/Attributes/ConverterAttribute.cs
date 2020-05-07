using System;
using System.Reflection;
using Velo.Serialization.Objects;
using Velo.Utils;

namespace Velo.Serialization.Attributes
{
    /// <summary>
    /// Custom property or class converter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ConverterAttribute : Attribute
    {
        public static bool IsDefined(Type type)
        {
            return Attribute.IsDefined(type, typeof(ConverterAttribute));
        }

        public static bool IsDefined(PropertyInfo property)
        {
            return Attribute.IsDefined(property, typeof(ConverterAttribute));
        }

        private readonly Type? _converterType;

        /// <summary>
        /// Set a custom converter for property or class
        /// </summary>
        public ConverterAttribute(Type converterType)
        {
            _converterType = converterType;

            if (ReflectionUtils.IsGenericInterfaceImplementation(converterType, typeof(IPropertyConverter<>)))
            {
                return;
            }

            if (ReflectionUtils.IsGenericInterfaceImplementation(converterType, typeof(IJsonConverter<>)))
            {
                return;
            }

            var typeName = ReflectionUtils.GetName(converterType);
            throw Error.InvalidOperation($"Type {typeName} must implement object or property converter interface");
        }

        protected ConverterAttribute()
        {
            _converterType = null;
        }

        public virtual Type GetConverterType(Type owner, PropertyInfo? property = null)
        {
            if (_converterType == null)
            {
                throw Error.InvalidOperation($"Setup {nameof(ConverterAttribute)}.{nameof(GetConverterType)}");
            }

            return _converterType.IsGenericTypeDefinition
                ? _converterType.MakeGenericType(owner)
                : _converterType;
        }
    }
}