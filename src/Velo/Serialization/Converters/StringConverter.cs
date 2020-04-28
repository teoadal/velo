using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class StringConverter : IJsonConverter<string>
    {
        public bool IsPrimitive => true;

        public string Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return token.Value!;
        }

        public string Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return jsonValue.Type == JsonDataType.Null
                ? null!
                : jsonValue.Value;
        }

        public void Serialize(string value, TextWriter writer)
        {
            if (value == null)
            {
                writer.Write(JsonTokenizer.TokenNullValue);
            }
            else
            {
                writer.Write('"');
                writer.Write(value);
                writer.Write('"');
            }
        }

        public JsonData Write(string value)
        {
            if (value == null) return JsonValue.Null;

            return string.IsNullOrEmpty(value)
                ? JsonValue.StringEmpty
                : new JsonValue(value, JsonDataType.String);
        }

        object IJsonConverter.DeserializeObject(ref JsonTokenizer tokenizer) => Deserialize(ref tokenizer);

        object IJsonConverter.ReadObject(JsonData data) => Read(data);

        void IJsonConverter.SerializeObject(object value, TextWriter writer) => Serialize((string) value, writer);

        JsonData IJsonConverter.WriteObject(object value) => Write((string) value);
    }
}