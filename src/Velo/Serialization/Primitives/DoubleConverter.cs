using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
{
    internal sealed class DoubleConverter : JsonConverter<double>
    {
        public const string Pattern = "0.0##########";

        private readonly CultureInfo _cultureInfo;

        public DoubleConverter(CultureInfo cultureInfo) : base(true)
        {
            _cultureInfo = cultureInfo;
        }

        public override double Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return double.Parse(token.Value, _cultureInfo);
        }

        public override double Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return double.Parse(jsonValue.Value, _cultureInfo);
        }

        public override void Serialize(double value, TextWriter output)
        {
            output.Write(value.ToString(Pattern, _cultureInfo));
        }

        public override JsonData Write(double value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.Number);
        }
    }
}