using Velo.Collections.Local;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Models
{
    internal static class JsonVisitor
    {
        public static JsonData Visit(JsonTokenizer tokenizer)
        {
            var current = tokenizer.Current;
            switch (current.TokenType)
            {
                case JsonTokenType.ArrayStart:
                    return VisitArray(tokenizer);
                case JsonTokenType.ObjectStart:
                    return VisitObject(tokenizer);
                default:
                    return VisitValue(current);
            }
        }

        public static JsonArray VisitArray(JsonTokenizer tokenizer)
        {
            var elements = new LocalList<JsonData>();

            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                if (current.TokenType == JsonTokenType.ArrayEnd) break;

                var element = Visit(tokenizer);
                elements.Add(element);
            }

            return elements.Length == 0
                ? JsonArray.Empty
                : new JsonArray(elements.ToArray());
        }

        public static JsonObject VisitObject(JsonTokenizer tokenizer)
        {
            var instance = new JsonObject();

            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                if (current.TokenType == JsonTokenType.ObjectEnd) break;

                var propertyValue = VisitProperty(tokenizer, out var propertyName);
                instance.Add(propertyName, propertyValue);
            }

            return instance;
        }

        public static JsonData VisitProperty(JsonTokenizer tokenizer)
        {
            tokenizer.MoveNext();
            return Visit(tokenizer);
        }

        public static JsonData VisitProperty(JsonTokenizer tokenizer, out string propertyName)
        {
            var property = tokenizer.Current.GetNotNullPropertyName();

            tokenizer.MoveNext();

            propertyName = property!;
            return Visit(tokenizer);
        }

        public static JsonValue VisitValue(JsonToken token)
        {
            switch (token.TokenType)
            {
                case JsonTokenType.False:
                    return JsonValue.False;
                case JsonTokenType.Number:
                    var value = token.Value!;
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
                    throw Error.Deserialization($"Invalid value token {token.TokenType}");
            }
        }
    }
}