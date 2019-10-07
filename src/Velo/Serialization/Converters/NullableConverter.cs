using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class NullableConverter<TNullable>: IJsonConverter<TNullable?>
        where TNullable: struct
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

        public void Serialize(TNullable? value, StringBuilder builder)
        {
            if (value == null) builder.Append(JsonTokenizer.TOKEN_NULL_VALUE);
            else _valueConverter.Serialize(value.Value, builder);
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((TNullable?) value, builder);
    }
}