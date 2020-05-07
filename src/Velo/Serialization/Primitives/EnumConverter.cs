using System;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
{
    internal sealed class EnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : Enum
    {
        private readonly TEnum[] _values;

        public EnumConverter() : base(true)
        {
            _values = (TEnum[]) Enum.GetValues(typeof(TEnum));
        }

        public override TEnum Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return _values[int.Parse(token.Value)];
        }

        public override TEnum Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return _values[int.Parse(jsonValue.Value)];
        }

        public override void Serialize(TEnum value, TextWriter output)
        {
            var index = Array.IndexOf(_values, value);
            output.Write(index);
        }

        public override JsonData Write(TEnum value)
        {
            var index = Array.IndexOf(_values, value);
            return new JsonValue(index.ToString(), JsonDataType.Number);
        }
    }
}