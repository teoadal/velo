using System;
using System.IO;
using System.Text;
using Velo.Collections;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Models
{
    internal abstract class JsonData
    {
        [ThreadStatic] private static StringBuilder _buffer;

        public static JsonData Parse(Stream stream, Encoding encoding = null)
        {
            var reader = new JsonReader(stream, encoding ?? Encoding.UTF8);
            var result = Parse(reader);
            reader.Dispose();

            return result;
        }

        public static JsonData Parse(string source)
        {
            var reader = new JsonReader(source);
            var result = Parse(reader);
            reader.Dispose();

            return result;
        }

        public readonly JsonDataType Type;

        protected JsonData(JsonDataType type)
        {
            Type = type;
        }

        private static JsonData Parse(JsonReader reader)
        {
            if (_buffer == null) _buffer = new StringBuilder(200);

            var tokenizer = new JsonTokenizer(reader, _buffer);

            tokenizer.MoveNext();

            var result = Visit(ref tokenizer);

            tokenizer.Dispose();

            return result;
        }

        private static JsonData Visit(ref JsonTokenizer tokenizer)
        {
            var current = tokenizer.Current;
            switch (current.TokenType)
            {
                case JsonTokenType.ArrayStart:
                    return VisitArray(ref tokenizer);
                case JsonTokenType.ObjectStart:
                    return VisitObject(ref tokenizer);
                default:
                    return VisitValue(current);
            }
        }

        private static JsonArray VisitArray(ref JsonTokenizer tokenizer)
        {
            var elements = new LocalList<JsonData>();

            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                if (current.TokenType == JsonTokenType.ArrayEnd) break;

                var element = Visit(ref tokenizer);
                elements.Add(element);
            }

            return elements.Length == 0
                ? JsonArray.Empty
                : new JsonArray(elements.ToArray());
        }

        private static JsonObject VisitObject(ref JsonTokenizer tokenizer)
        {
            var instance = new JsonObject();

            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                if (current.TokenType == JsonTokenType.ObjectEnd) break;

                var property = current.Value;

                tokenizer.MoveNext();

                instance.Add(property, Visit(ref tokenizer));
            }

            return instance;
        }

        private static JsonValue VisitValue(JsonToken token)
        {
            switch (token.TokenType)
            {
                case JsonTokenType.False:
                    return JsonValue.False;
                case JsonTokenType.Number:
                    var value = token.Value;
                    return value.Length == 1 && value[0] == '0'
                        ? JsonValue.Zero
                        : new JsonValue(value, JsonDataType.Number);
                case JsonTokenType.Null:
                    return JsonValue.Null;
                case JsonTokenType.String:
                    return JsonValue.String(token.Value);
                case JsonTokenType.True:
                    return JsonValue.True;
                default:
                    throw Error.InvalidData($"Invalid value token {token.TokenType}");
            }
        }
    }
}