using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization
{
    internal static class SerializationExtensions
    {
        public static string GetNotNullPropertyName(this JsonToken token)
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

        public static object Deserialize(this IJsonConverter converter, string json, StringBuilder? sb = null)
        {
            using var tokenizer = new JsonTokenizer(json, sb ?? new StringBuilder());

            if (converter.IsPrimitive) tokenizer.MoveNext();
            return converter.DeserializeObject(tokenizer);
        }

        public static T Deserialize<T>(this IJsonConverter<T> converter, string json, StringBuilder? sb = null)
        {
            using var tokenizer = new JsonTokenizer(json, sb ?? new StringBuilder());

            if (converter.IsPrimitive) tokenizer.MoveNext();
            return converter.Deserialize(tokenizer);
        }

        public static T Read<T>(this IConvertersCollection converters, JsonData json)
        {
            return converters.Get<T>().Read(json);
        }

        public static string Serialize(this JsonData data)
        {
            var stringWriter = new StringWriter();
            data.Serialize(stringWriter);
            return stringWriter.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TextWriter WriteProperty(this TextWriter writer, string propertyName)
        {
            writer.Write('"');
            writer.Write(propertyName);
            writer.Write("\":");

            return writer;
        }

        public static TextWriter WriteProperty(this TextWriter writer, string propertyName, JsonData propertyValue)
        {
            WriteProperty(writer, propertyName);

            propertyValue.Serialize(writer);

            return writer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TextWriter WriteString(this TextWriter writer, string value)
        {
            writer.Write('"');
            writer.Write(value);
            writer.Write('"');

            return writer;
        }
    }
}