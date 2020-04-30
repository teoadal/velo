using System;
using System.Globalization;
using System.IO;
using System.Text;
using Velo.Serialization.Models;
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
            using var reader = new JsonReader(source);
            return Deserialize<TOut>(reader);
        }

        public TOut Deserialize<TOut>(Stream source, Encoding? encoding = null)
        {
            using var reader = new JsonReader(source, encoding ?? Encoding.UTF8);
            return Deserialize<TOut>(reader);
        }

        public string Serialize(object source)
        {
            if (source == null) return JsonValue.NullToken;

            _buffer ??= new StringBuilder(200);

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
                writer.Write(JsonValue.NullToken);
                return;
            }
            
            var type = source.GetType();
            var converter = _converters.Get(type);
            
            converter.SerializeObject(source, writer);
        }
        
        private TOut Deserialize<TOut>(JsonReader reader)
        {
            _buffer ??= new StringBuilder(200);

            var converter = _converters.Get<TOut>();

            using var tokenizer = new JsonTokenizer(reader, _buffer);
            if (converter.IsPrimitive) tokenizer.MoveNext();

            return converter.Deserialize(tokenizer);
        }
    }
}