using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
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

            // ReSharper disable once AssignNullToNotNullAttribute
            return jsonValue.Type == JsonDataType.Null
                ? null!
                : jsonValue.Value!;
        }

        public override void Serialize(string? value, TextWriter output)
        {
            if (value == null)
            {
                output.Write(JsonValue.NullToken);
            }
            else
            {
                output.WriteString(value);
            }
        }

        public override JsonData Write(string? value)
        {
            if (value == null) return JsonValue.Null;

            return string.IsNullOrEmpty(value)
                ? JsonValue.StringEmpty
                : new JsonValue(value, JsonDataType.String);
        }
    }
}