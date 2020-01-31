using System.Text;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal sealed class BoolConverter : IJsonConverter<bool>
    {
        public bool IsPrimitive => true;

        public bool Deserialize(ref JsonTokenizer tokenizer)
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

        public bool Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return jsonValue.Type == JsonDataType.True;
        }

        public void Serialize(bool value, StringBuilder builder)
        {
            builder.Append(value ? JsonTokenizer.TokenTrueValue : JsonTokenizer.TokenFalseValue);
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((bool) value, builder);
    }
}