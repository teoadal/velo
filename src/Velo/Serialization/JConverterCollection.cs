using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Velo.Serialization.Converters;
using Velo.Utils;

namespace Velo.Serialization
{
    internal sealed class JConverterCollection : ConcurrentDictionary<Type, IJsonConverter>
    {
        private readonly Func<Type, IJsonConverter> _buildConverter;
        private readonly Type _listGenericType;

        public JConverterCollection(CultureInfo culture) : base(new[]
        {
            new KeyValuePair<Type, IJsonConverter>(typeof(bool), new BoolConverter()),
            new KeyValuePair<Type, IJsonConverter>(typeof(DateTime), new DateTimeConverter(culture)),
            new KeyValuePair<Type, IJsonConverter>(typeof(double), new DoubleConverter(culture)),
            new KeyValuePair<Type, IJsonConverter>(typeof(float), new FloatConverter(culture)),
            new KeyValuePair<Type, IJsonConverter>(typeof(Guid), new GuidConverter()),
            new KeyValuePair<Type, IJsonConverter>(typeof(int), new IntConverter()),
            new KeyValuePair<Type, IJsonConverter>(typeof(string), new StringConverter()),
        })
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            _buildConverter = t => Build(t);
            _listGenericType = typeof(List<>);
        }

        public IJsonConverter Get(Type type)
        {
            return GetOrAdd(type, _buildConverter);
        }

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
            var elementConverter = GetOrAdd(elementType, _buildConverter);

            var converterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }

        private IJsonConverter BuildEnumConverter(Type type)
        {
            var converterType = typeof(EnumConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType);
        }

        private IJsonConverter BuildListConverter(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var elementConverter = GetOrAdd(elementType, _buildConverter);

            var converterType = typeof(ListConverter<>).MakeGenericType(elementType);
            return (IJsonConverter) Activator.CreateInstance(converterType, elementConverter);
        }
        
        private IJsonConverter BuildNullableConverter(Type underlyingType)
        {
            var valueConverter = GetOrAdd(underlyingType, _buildConverter);
            var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(underlyingType);
            return (IJsonConverter) Activator.CreateInstance(nullableConverterType, valueConverter);
        }

        private IJsonConverter BuildObjectConverter(Type type)
        {
            var properties = type.GetProperties();
            var propertyConverters = new Dictionary<PropertyInfo, IJsonConverter>(properties.Length);

            foreach (var property in properties)
            {
                var propertyConverter = GetOrAdd(property.PropertyType, _buildConverter);
                propertyConverters.Add(property, propertyConverter);
            }

            var converterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(converterType, propertyConverters);
        }
    }
}