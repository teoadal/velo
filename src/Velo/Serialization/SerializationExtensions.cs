using System.IO;
using System.Text;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization
{
    public static class SerializationExtensions
    {
        internal static string GetNotNullPropertyName(this JsonToken token)
        {
            var tokenType = token.TokenType;

            if (tokenType != JsonTokenType.Property)
            {
                throw Error.Deserialization(JsonTokenType.Property, tokenType);
            }

            var propertyName = token.Value;

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw Error.Deserialization($"Expected not empty {JsonTokenType.Property} token");
            }

            return propertyName!;
        }

        internal static T Deserialize<T>(this IJsonConverter<T> converter, string json, StringBuilder? sb = null)
        {
            using var reader = new JsonReader(json);
            using var tokenizer = new JsonTokenizer(reader, sb ?? new StringBuilder());

            if (converter.IsPrimitive) tokenizer.MoveNext();
            return converter.Deserialize(tokenizer);
        }

        internal static T Read<T>(this IConvertersCollection converters, JsonData json)
        {
            return converters.Get<T>().Read(json);
        }

        public static string Serialize(this JsonData data)
        {
            var stringWriter = new StringWriter();
            data.Serialize(stringWriter);
            return stringWriter.ToString();
        }
    }
}