using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class DoubleConverter : IJsonConverter<double>
    {
        public bool IsPrimitive => true;

        private readonly CultureInfo _cultureInfo;

        public DoubleConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public double Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return double.Parse(token.Value, _cultureInfo);
        }

        public double Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return double.Parse(jsonValue.Value, _cultureInfo);
        }

        public void Serialize(double value, TextWriter builder)
        {
            builder.Write(value.ToString(_cultureInfo));
        }

        public JsonData Write(double value)
        {
            return new JsonValue(value.ToString(_cultureInfo), JsonDataType.Number);
        }

        void IJsonConverter.Serialize(object value, TextWriter builder) => Serialize((double) value, builder);

        JsonData IJsonConverter.Write(object value) => Write((double) value);
    }
}