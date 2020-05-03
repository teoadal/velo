using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal sealed class BoolConverter : JsonConverter<bool>
    {
        public BoolConverter() : base(true)
        {
        }

        public override bool Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;

            switch (token.TokenType)
            {
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                default:
                    throw Error.InvalidOperation($"Invalid boolean token '{token.TokenType}'");
            }
        }

        public override bool Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return jsonValue.Type == JsonDataType.True;
        }

        public override void Serialize(bool value, TextWriter writer)
        {
            writer.Write(value ? JsonValue.TrueToken : JsonValue.FalseToken);
        }

        public override JsonData Write(bool value)
        {
            return value ? JsonValue.True : JsonValue.False;
        }
    }
}