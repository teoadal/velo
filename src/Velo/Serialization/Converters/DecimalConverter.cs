using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class DecimalConverter : IJsonConverter<decimal>
    {
        public const string Pattern = "0.0";

        public bool IsPrimitive => true;

        private readonly CultureInfo _cultureInfo;

        public DecimalConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public decimal Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return decimal.Parse(token.Value, _cultureInfo);
        }

        public decimal Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return decimal.Parse(jsonValue.Value, _cultureInfo);
        }

        public void Serialize(decimal value, TextWriter builder)
        {
            builder.Write(value.ToString(Pattern));
        }

        public JsonData Write(decimal value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.Number);
        }

        object IJsonConverter.DeserializeObject(JsonTokenizer tokenizer) => Deserialize(tokenizer);

        object IJsonConverter.ReadObject(JsonData data) => Read(data);

        void IJsonConverter.SerializeObject(object value, TextWriter builder) => Serialize((decimal) value, builder);

        JsonData IJsonConverter.WriteObject(object value) => Write((decimal) value);
    }
}