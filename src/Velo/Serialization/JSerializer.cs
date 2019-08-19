using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Velo.Serialization.Converters;

namespace Velo.Serialization
{
    public sealed class JSerializer
    {
        private static StringBuilder _buffer;
        private readonly Dictionary<Type, IJsonConverter> _converters;

        public JSerializer(CultureInfo culture = null)
        {
            if (culture == null) culture = CultureInfo.InvariantCulture;

            _converters = new Dictionary<Type, IJsonConverter>
            {
                {typeof(bool), new BoolConverter()},
                {typeof(double), new DoubleConverter(culture)},
                {typeof(float), new FloatConverter(culture)},
                {typeof(int), new IntConverter()},
                {typeof(string), new StringConverter()},
            };
        }

        public T Deserialize<T>(string source)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var type = typeof(T);
            var converter = GetOrBuildConverter(type);
            using (var tokenizer = new JsonTokenizer(source, _buffer))
            {
                if (type.IsPrimitive) tokenizer.MoveNext();

                var typedConverter = (IJsonConverter<T>) converter;
                return typedConverter.Convert(tokenizer);
            }
        }

        public void PrepareForSource<TSource>()
        {
            var sourceType = typeof(TSource);

            if (!_converters.ContainsKey(sourceType)) return;

            var converter = BuildConverter(sourceType);
            _converters.Add(sourceType, converter);
        }

        internal IJsonConverter GetOrBuildConverter(Type type)
        {
            if (_converters.TryGetValue(type, out var exists)) return exists;

            var converter = BuildConverter(type);
            _converters.Add(type, converter);

            return converter;
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

            var objectConverterType = typeof(ObjectConverter<>).MakeGenericType(type);
            return (IJsonConverter) Activator.CreateInstance(objectConverterType, this);
        }
    }
}