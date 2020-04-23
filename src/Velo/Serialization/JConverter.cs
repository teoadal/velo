using System;
using System.Globalization;
using System.IO;
using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization
{
    public sealed class JConverter
    {
        [ThreadStatic] 
        private static StringBuilder? _buffer;

        private readonly IConvertersCollection _converters;

        public JConverter(CultureInfo? culture = null)
        {
            _converters = new ConvertersCollection(culture ?? CultureInfo.InvariantCulture);
        }

        internal JConverter(IConvertersCollection convertersCollection)
        {
            _converters = convertersCollection;
        }
        
        public TOut Deserialize<TOut>(string source)
        {
            var reader = new JsonReader(source);
            var result = Deserialize<TOut>(reader);
            reader.Dispose();

            return result;
        }

        public TOut Deserialize<TOut>(Stream source, Encoding? encoding = null)
        {
            var reader = new JsonReader(source, encoding ?? Encoding.UTF8);
            var result = Deserialize<TOut>(reader);
            reader.Dispose();

            return result;
        }

        public string Serialize(object source)
        {
            if (source == null) return JsonTokenizer.TokenNullValue;

            if (_buffer == null) _buffer = new StringBuilder(200);

            using var stringWriter = new StringWriter(_buffer);
            
            Serialize(source, stringWriter);
            
            var json = _buffer.ToString();
            _buffer.Clear();
            return json;
        }

        public void Serialize(object source, TextWriter writer)
        {
            if (source == null)
            {
                writer.Write(JsonTokenizer.TokenNullValue);
                return;
            }
            
            var type = source.GetType();
            var converter = _converters.Get(type);
            
            converter.SerializeObject(source, writer);
        }
        
        private TOut Deserialize<TOut>(JsonReader reader)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var converter = _converters.Get<TOut>();

            var tokenizer = new JsonTokenizer(reader, _buffer);
            if (converter.IsPrimitive) tokenizer.MoveNext();

            var result = converter.Deserialize(ref tokenizer);

            tokenizer.Dispose();

            return result;
        }
    }
}