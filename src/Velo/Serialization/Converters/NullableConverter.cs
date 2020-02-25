using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class NullableConverter<TNullable> : IJsonConverter<TNullable?>
        where TNullable : struct
    {
        public bool IsPrimitive => true;

        private readonly IJsonConverter<TNullable> _valueConverter;

        public NullableConverter(IJsonConverter<TNullable> valueConverter)
        {
            _valueConverter = valueConverter;
        }

        public TNullable? Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            if (token.TokenType == JsonTokenType.Null) return null;
            return _valueConverter.Deserialize(ref tokenizer);
        }

        public TNullable? Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            if (jsonValue.Type == JsonDataType.Null) return null;
            return _valueConverter.Read(jsonData);
        }

        public void Serialize(TNullable? value, TextWriter writer)
        {
            if (value == null) writer.Write(JsonTokenizer.TokenNullValue);
            else _valueConverter.Serialize(value.Value, writer);
        }

        public JsonData Write(TNullable? value)
        {
            return value == null
                ? JsonValue.Null
                : _valueConverter.Write(value.Value);
        }

        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((TNullable?) value, writer);

        JsonData IJsonConverter.Write(object value) => Write((TNullable?) value);
    }
}