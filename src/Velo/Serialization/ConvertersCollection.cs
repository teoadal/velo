using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Velo.Serialization.Converters;
using Velo.Utils;

namespace Velo.Serialization
{
    internal interface IConvertersCollection
    {
        IJsonConverter Get(Type type);

        IJsonConverter<T> Get<T>();
    }

    internal sealed class ConvertersCollection : IConvertersCollection
    {
        private readonly Dictionary<Type, IJsonConverter> _converters;
        private readonly Type _listGenericType;
        private readonly object _lock;

        public ConvertersCollection(CultureInfo culture)
        {
            _converters = BuildDefaultConverters(culture);
            _listGenericType = typeof(List<>);
            _lock = new object();
        }

        public IJsonConverter Get(Type type)
        {
            if (_converters.TryGetValue(type, out var exists)) return exists;

            var converter = Build(type);

            using (Lock.Enter(_lock))
            {
                _converters[type] = converter;
            }

            return converter;
        }

        public IJsonConverter<T> Get<T>() => (IJsonConverter<T>) Get(Typeof<T>.Raw);

        private IJsonConverter Build(Type type)
        {
            if (type.IsArray) return BuildArrayConverter(type);

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null) return BuildNullableConverter(underlyingType);

            if (type.IsEnum) return BuildEnumConverter(type);

            return ReflectionUtils.IsGenericTypeImplementation(type, _listGenericType)
                ? BuildListConverter(type)
                : BuildObjectConverter(type);
        }

        private IJsonConverter BuildArrayConverter(Type type)
        {
            var elementType = ReflectionUtils.GetArrayElementType(type);
            var elementConverter = Get(elementType);

            var converterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }

        private static IJsonConverter BuildEnumConverter(Type type)
        {
            var converterType = typeof(EnumConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType);
        }

        private IJsonConverter BuildListConverter(Type type)
        {
            var elementType = type.GenericTypeArguments[0];
            var elementConverter = Get(elementType);

            var converterType = typeof(ListConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }

        private IJsonConverter BuildNullableConverter(Type underlyingType)
        {
            var valueConverter = Get(underlyingType);
            var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(underlyingType);
            return (IJsonConverter) Activator.CreateInstance(nullableConverterType, valueConverter);
        }

        private IJsonConverter BuildObjectConverter(Type type)
        {
            var properties = type.GetProperties();
            var propertyConverters = new (PropertyInfo, IJsonConverter)[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var propertyConverter = Get(property.PropertyType);
                propertyConverters[i] = (property, propertyConverter);
            }

            var converterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType, propertyConverters);
        }

        private static Dictionary<Type, IJsonConverter> BuildDefaultConverters(CultureInfo culture)
        {
            return new Dictionary<Type, IJsonConverter>
            {
                {typeof(bool), new BoolConverter()},
                {typeof(DateTime), new DateTimeConverter(culture)},
                {typeof(double), new DoubleConverter(culture)},
                {typeof(float), new FloatConverter(culture)},
                {typeof(Guid), new GuidConverter()},
                {typeof(int), new IntConverter()},
                {typeof(string), new StringConverter()},
                {typeof(TimeSpan), new TimeSpanConverter(culture)}
            };
        }
    }
}