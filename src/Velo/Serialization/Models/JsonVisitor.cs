using Velo.Collections;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Models
{
    internal static class JsonVisitor
    {
        public static JsonData Visit(ref JsonTokenizer tokenizer)
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

        public static JsonArray VisitArray(ref JsonTokenizer tokenizer)
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

        public static JsonObject VisitObject(ref JsonTokenizer tokenizer)
        {
            var instance = new JsonObject();

            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                if (current.TokenType == JsonTokenType.ObjectEnd) break;

                var propertyValue = VisitProperty(ref tokenizer, out var propertyName);
                instance.Add(propertyName, propertyValue);
            }

            return instance;
        }

        public static JsonData VisitProperty(ref JsonTokenizer tokenizer)
        {
            tokenizer.MoveNext();
            return Visit(ref tokenizer);
        }

        public static JsonData VisitProperty(ref JsonTokenizer tokenizer, out string propertyName)
        {
            var property = tokenizer.Current.GetNotNullPropertyName();

            tokenizer.MoveNext();

            propertyName = property!;
            return Visit(ref tokenizer);
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