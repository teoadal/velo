using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class StringConverter : JsonConverter<string>
    {
        public StringConverter() : base(true)
        {
        }

        public override string Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return token.Value!;
        }

        public override string Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return jsonValue.Type == JsonDataType.Null
                ? null!
                : jsonValue.Value;
        }

        public override void Serialize(string value, TextWriter writer)
        {
            if (value == null)
            {
                writer.Write(JsonValue.NullToken);
            }
            else
            {
                writer.Write('"');
                writer.Write(value);
                writer.Write('"');
            }
        }

        public override JsonData Write(string value)
        {
            if (value == null) return JsonValue.Null;

            return string.IsNullOrEmpty(value)
                ? JsonValue.StringEmpty
                : new JsonValue(value, JsonDataType.String);
        }
    }
}