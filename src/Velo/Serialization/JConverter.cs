using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Velo.Serialization.Converters;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization
{
    public sealed class JConverter
    {
        [ThreadStatic] private static StringBuilder _buffer;

        private readonly Func<Type, IJsonConverter> _buildConverter;
        private readonly ConcurrentDictionary<Type, IJsonConverter> _converters;

        public JConverter(CultureInfo culture = null)
        {
            if (culture == null) culture = CultureInfo.InvariantCulture;

            _buildConverter = BuildConverter;
            _converters = new ConcurrentDictionary<Type, IJsonConverter>(new[]
            {
                new KeyValuePair<Type, IJsonConverter>(typeof(bool), new BoolConverter()),
                new KeyValuePair<Type, IJsonConverter>(typeof(DateTime), new DateTimeConverter(culture)),
                new KeyValuePair<Type, IJsonConverter>(typeof(double), new DoubleConverter(culture)),
                new KeyValuePair<Type, IJsonConverter>(typeof(float), new FloatConverter(culture)),
                new KeyValuePair<Type, IJsonConverter>(typeof(Guid), new GuidConverter()),
                new KeyValuePair<Type, IJsonConverter>(typeof(int), new IntConverter()),
                new KeyValuePair<Type, IJsonConverter>(typeof(string), new StringConverter()),
            });
        }

        public TOut Deserialize<TOut>(string source)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var outType = typeof(TOut);
            var converter = _converters.GetOrAdd(outType, _buildConverter);
            
            using (var tokenizer = new JsonTokenizer(source, _buffer))
            {
                if (converter.IsPrimitive) tokenizer.MoveNext();

                var typedConverter = (IJsonConverter<TOut>) converter;
                return typedConverter.Deserialize(tokenizer);
            }
        }

        public string Serialize(object source)
        {
            if (source == null) return JsonTokenizer.TOKEN_NULL_VALUE;
            if (_buffer == null) _buffer = new StringBuilder(200);

            var type = source.GetType();
            var converter = _converters.GetOrAdd(type, BuildConverter);

            converter.Serialize(source, _buffer);

            var json = _buffer.ToString();
            _buffer.Clear();
            return json;
        }

        public void PrepareConverterFor<TSource>()
        {
            var sourceType = typeof(TSource);
            _converters.GetOrAdd(sourceType, _buildConverter);
        }

        private IJsonConverter BuildConverter(Type type)
        {
            if (type.IsArray)
            {
                var elementType = ReflectionUtils.GetArrayElementType(type);
                var elementConverter = _converters.GetOrAdd(elementType, _buildConverter);

                var arrayConverterType = typeof(ArrayConverter<>).MakeGenericType(elementType);
                return (IJsonConverter) Activator.CreateInstance(arrayConverterType, elementConverter);
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                var valueConverter = _converters.GetOrAdd(underlyingType, _buildConverter);
                var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(underlyingType);
                return (IJsonConverter) Activator.CreateInstance(nullableConverterType, valueConverter);
            }
            
            if (type.IsEnum)
            {
                var enumConverterType = typeof(EnumConverter<>).MakeGenericType(type);
                return (IJsonConverter) Activator.CreateInstance(enumConverterType);
            }
            
            if (ReflectionUtils.IsGenericTypeImplementation(type, typeof(List<>)))
            {
                var listElementType = type.GetGenericArguments()[0];
                var listElementConverter = _converters.GetOrAdd(listElementType, _buildConverter);

                var listConverterType = typeof(ListConverter<>).MakeGenericType(listElementType);
                return (IJsonConverter) Activator.CreateInstance(listConverterType, listElementConverter);
            }
            
            var objectProperties = type.GetProperties();
            var objectPropertyConverters = new Dictionary<PropertyInfo, IJsonConverter>(objectProperties.Length);
            
            for (var i = 0; i < objectProperties.Length; i++)
            {
                var property = objectProperties[i];
                var propertyConverter = _converters.GetOrAdd(property.PropertyType, _buildConverter);
                objectPropertyConverters.Add(property, propertyConverter);
            }

            var objectConverterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(objectConverterType, objectPropertyConverters);
        }
    }
}