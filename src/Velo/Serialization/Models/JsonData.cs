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
            _buffer ??= new StringBuilder(200);

            using var tokenizer = new JsonTokenizer(new JsonReader(stream, encoding), _buffer);
            tokenizer.MoveNext();
            return JsonVisitor.Visit(tokenizer);
        }

        public static JsonData Parse(string json)
        {
            _buffer ??= new StringBuilder(200);

            using var tokenizer = new JsonTokenizer(json, _buffer);
            tokenizer.MoveNext();
            return JsonVisitor.Visit(tokenizer);
        }

        public readonly JsonDataType Type;

        protected JsonData(JsonDataType type)
        {
            Type = type;
        }

        public abstract void Serialize(TextWriter writer);
    }
}