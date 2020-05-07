using System;
using System.Globalization;
using System.IO;
using System.Text;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization
{
    public sealed class JConverter
    {
        internal readonly IConvertersCollection Converters;

        [ThreadStatic]
        private static StringBuilder? _buffer;

        #region Constructors

        internal JConverter(CultureInfo? culture = null, IConvertersCollection? converters = null)
        {
            Converters = converters ?? new ConvertersCollection(culture ?? CultureInfo.InvariantCulture);
        }

        internal JConverter(IConvertersCollection convertersCollection)
        {
            Converters = convertersCollection;
        }

        #endregion

        public TOut Deserialize<TOut>(string? source)
        {
            if (source == null) throw Error.Null(nameof(source));

            using var reader = new JsonReader(source);
            return Deserialize<TOut>(reader);
        }

        public TOut Deserialize<TOut>(Stream source, Encoding? encoding = null)
        {
            using var reader = new JsonReader(source, encoding ?? Encoding.UTF8);
            return Deserialize<TOut>(reader);
        }

        public string Serialize(object? source)
        {
            if (source == null) return JsonValue.NullToken;

            _buffer ??= new StringBuilder(200);

            using var stringWriter = new StringWriter(_buffer);

            Serialize(source, stringWriter);

            var json = _buffer.ToString();
            _buffer.Clear();
            return json;
        }

        public void Serialize(object? source, TextWriter writer)
        {
            if (source == null)
            {
                writer.Write(JsonValue.NullToken);
                return;
            }

            var type = source.GetType();
            var converter = Converters.Get(type);

            converter.SerializeObject(source, writer);
        }

        private TOut Deserialize<TOut>(JsonReader reader)
        {
            _buffer ??= new StringBuilder(200);

            var converter = Converters.Get<TOut>();

            using var tokenizer = new JsonTokenizer(reader, _buffer);
            if (converter.IsPrimitive) tokenizer.MoveNext();

            return converter.Deserialize(tokenizer);
        }
    }
}