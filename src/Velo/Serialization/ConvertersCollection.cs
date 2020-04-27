using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Velo.Collections;
using Velo.Serialization.Converters;
using Velo.Utils;

namespace Velo.Serialization
{
    internal sealed class ConvertersCollection : DangerousVector<Type, IJsonConverter>, IConvertersCollection
    {
        private readonly Func<Type, IJsonConverter> _builder;

        private readonly Type[] _arrayLikeGenericTypes;
        private readonly Type[] _listGenericTypes;

        public ConvertersCollection(CultureInfo culture)
            : base(BuildDefaultConverters(culture))
        {
            _builder = Build;
            _arrayLikeGenericTypes = new[]
            {
                typeof(ICollection<>),
                typeof(IEnumerable<>),
                typeof(IReadOnlyCollection<>),
            };
            _listGenericTypes = new[]
            {
                typeof(List<>),
                typeof(IList<>)
            };
        }

        public IJsonConverter Get(Type type)
        {
            return GetOrAdd(type, _builder);
        }

        public IJsonConverter<T> Get<T>()
        {
            return (IJsonConverter<T>) GetOrAdd(Typeof<T>.Raw, _builder);
        }

        private IJsonConverter Build(Type type)
        {
            if (type.IsArray) return BuildArrayConverter(type);

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null) return BuildNullableConverter(underlyingType);

            if (type.IsEnum) return BuildEnumConverter(type);

            if (_arrayLikeGenericTypes.Contains((arrayLike, test) => ReflectionUtils.IsGenericTypeImplementation(test, arrayLike), type))
            {
                return BuildArrayLikeConverter(type);
            }

            return _listGenericTypes.Contains((arrayLike, test) => ReflectionUtils.IsGenericTypeImplementation(test, arrayLike), type)
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

        private IJsonConverter BuildArrayLikeConverter(Type type)
        {
            var elementType = type.GenericTypeArguments[0];
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