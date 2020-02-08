using System;
using System.IO;
using Velo.Serialization.Models;
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

        public TEnum Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return _values[int.Parse(token.Value)];
        }

        public TEnum Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData; 
            return _values[int.Parse(jsonValue.Value)];
        }
        
        public void Serialize(TEnum value, TextWriter writer)
        {
            var index = Array.IndexOf(_values, value);
            writer.Write(index);
        }
        
        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((TEnum) value, writer);
    }
}