using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
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

        public TNullable? Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            if (token.TokenType == JsonTokenType.Null) return null;
            return _valueConverter.Deserialize(tokenizer);
        }

        public TNullable? Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            if (jsonValue.Type == JsonDataType.Null) return null;
            return _valueConverter.Read(jsonData);
        }

        public void Serialize(TNullable? value, TextWriter output)
        {
            if (value == null) output.Write(JsonValue.NullToken);
            else _valueConverter.Serialize(value.Value, output);
        }

        public JsonData Write(TNullable? value)
        {
            return value == null
                ? JsonValue.Null
                : _valueConverter.Write(value.Value);
        }

        object IJsonConverter.DeserializeObject(JsonTokenizer tokenizer) => Deserialize(tokenizer)!;
        
        object IJsonConverter.ReadObject(JsonData data) => Read(data)!;

        void IJsonConverter.SerializeObject(object value, TextWriter writer) => Serialize((TNullable?) value, writer);

        JsonData IJsonConverter.WriteObject(object value) => Write((TNullable?) value);
    }
}