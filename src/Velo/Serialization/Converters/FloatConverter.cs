using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class FloatConverter : JsonConverter<float>
    {
        public const string Pattern = "0.0##########";

        private readonly CultureInfo _cultureInfo;

        public FloatConverter(CultureInfo cultureInfo) : base(true)
        {
            _cultureInfo = cultureInfo;
        }

        public override float Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return float.Parse(token.Value, _cultureInfo);
        }

        public override float Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return float.Parse(jsonValue.Value, _cultureInfo);
        }

        public override void Serialize(float value, TextWriter writer)
        {
            writer.Write(value.ToString(Pattern, _cultureInfo));
        }

        public override JsonData Write(float value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.Number);
        }
    }
}