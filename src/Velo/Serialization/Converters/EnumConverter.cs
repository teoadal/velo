using System;
using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class EnumConverter<TEnum> : IJsonConverter<TEnum>
        where TEnum: Enum
    {
        public bool IsPrimitive => true;

        private readonly TEnum[] _values;

        public EnumConverter()
        {
            _values = (TEnum[]) Enum.GetValues(typeof(TEnum));
        }

        public TEnum Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return _values[int.Parse(token.Value)];
        }

        public void Serialize(TEnum value, StringBuilder builder)
        {
            var index = Array.IndexOf(_values, value);
            builder.Append(index);
        }
        
        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((TEnum) value, builder);
    }
}