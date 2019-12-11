using System;
using System.Globalization;
using System.Text;
using Velo.Serialization.Converters;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization
{
    public sealed class JConverter
    {
        [ThreadStatic] 
        private static StringBuilder _buffer;

        private readonly JConverterCollection _converters;

        public JConverter(CultureInfo culture = null)
        {
            _converters = new JConverterCollection(culture ?? CultureInfo.InvariantCulture);
        }

        public TOut Deserialize<TOut>(string source)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var outType = typeof(TOut);
            var converter = _converters.Get(outType);

            var tokenizer = new JsonTokenizer(source, _buffer);
            if (converter.IsPrimitive) tokenizer.MoveNext();

            var typedConverter = (IJsonConverter<TOut>) converter;
            var result = typedConverter.Deserialize(ref tokenizer);

            tokenizer.Dispose();

            return result;
        }

        public string Serialize(object source)
        {
            if (source == null) return JsonTokenizer.TokenNullValue;

            if (_buffer == null) _buffer = new StringBuilder(200);

            var type = source.GetType();
            var converter = _converters.Get(type);

            converter.Serialize(source, _buffer);

            var json = _buffer.ToString();
            _buffer.Clear();
            return json;
        }

        public void PrepareConverterFor<TSource>()
        {
            var sourceType = typeof(TSource);
            _converters.Get(sourceType);
        }
    }
}