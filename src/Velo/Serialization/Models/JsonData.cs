using System;
using System.IO;
using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Models
{
    public abstract class JsonData
    {
        [ThreadStatic] 
        private static StringBuilder? _buffer;

        public static JsonData Parse(Stream stream, Encoding? encoding = null)
        {
            using var reader = new JsonReader(stream, encoding ?? Encoding.UTF8);
            return Parse(reader);
        }

        public static JsonData Parse(string source)
        {
            using var reader = new JsonReader(source);
            return Parse(reader);
        }

        public readonly JsonDataType Type;

        protected JsonData(JsonDataType type)
        {
            Type = type;
        }

        public abstract void Serialize(TextWriter writer);

        private static JsonData Parse(JsonReader reader)
        {
            _buffer ??= new StringBuilder(200);

            var tokenizer = new JsonTokenizer(reader, _buffer);

            tokenizer.MoveNext();

            var result = JsonVisitor.Visit(ref tokenizer);

            tokenizer.Dispose();

            return result;
        }
    }
}