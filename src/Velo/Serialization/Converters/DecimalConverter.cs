using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class DecimalConverter : JsonConverter<decimal>
    {
        public const string Pattern = "0.0";

        private readonly CultureInfo _cultureInfo;

        public DecimalConverter(CultureInfo cultureInfo): base(true)
        {
            _cultureInfo = cultureInfo;
        }

        public override decimal Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return decimal.Parse(token.Value, _cultureInfo);
        }

        public override decimal Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return decimal.Parse(jsonValue.Value, _cultureInfo);
        }

        public override void Serialize(decimal value, TextWriter builder)
        {
            builder.Write(value.ToString(Pattern));
        }

        public override JsonData Write(decimal value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.Number);
        }
    }
}