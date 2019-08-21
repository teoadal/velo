using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Velo.Serialization.Converters;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization
{
    public sealed class JConverter
    {
        [ThreadStatic] 
        private static StringBuilder _buffer;
        private readonly Dictionary<Type, IJsonConverter> _converters;

        public JConverter(CultureInfo culture = null)
        {
            if (culture == null) culture = CultureInfo.InvariantCulture;

            _converters = new Dictionary<Type, IJsonConverter>
            {
                {typeof(bool), new BoolConverter()},
                {typeof(DateTime), new DateTimeConverter(culture)},
                {typeof(double), new DoubleConverter(culture)},
                {typeof(float), new FloatConverter(culture)},
                {typeof(Guid), new GuidConverter()},
                {typeof(int), new IntConverter()},
                {typeof(string), new StringConverter()}
            };
        }

        public TOut Deserialize<TOut>(string source)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var outType = typeof(TOut);
            var converter = GetOrBuildConverter(outType);
            using (var tokenizer = new JsonTokenizer(source, _buffer))
            {
                if (outType.IsPrimitive || outType == typeof(string)) tokenizer.MoveNext();

                var typedConverter = (IJsonConverter<TOut>) converter;
                return typedConverter.Deserialize(tokenizer);
            }
        }

        public string Serialize(object source)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var type = source.GetType();
            var converter = GetOrBuildConverter(type);

            converter.Serialize(source, _buffer);

            var json = _buffer.ToString();
            _buffer.Clear();
            return json;
        }

        public void PrepareConverterFor<TSource>()
        {
            var sourceType = typeof(TSource);

            if (!_converters.ContainsKey(sourceType)) return;

            var converter = BuildConverter(sourceType);
            _converters.Add(sourceType, converter);
        }

        private IJsonConverter BuildConverter(Type type)
        {
            if (type.IsArray)
            {
                var arrayElementType = type.GetElementType();
                var arrayElementConverter = GetOrBuildConverter(arrayElementType);

                var arrayConverterType = typeof(ArrayConverter<>).MakeGenericType(arrayElementType);
                return (IJsonConverter) Activator.CreateInstance(arrayConverterType, arrayElementConverter);
            }

            var objectPropertyConverters = type
                .GetProperties()
                .ToDictionary(p => p, p => GetOrBuildConverter(p.PropertyType));

            var objectConverterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(objectConverterType, objectPropertyConverters);
        }

        private IJsonConverter GetOrBuildConverter(Type type)
        {
            if (_converters.TryGetValue(type, out var exists)) return exists;

            var converter = BuildConverter(type);
            _converters.Add(type, converter);

            return converter;
        }
    }
}