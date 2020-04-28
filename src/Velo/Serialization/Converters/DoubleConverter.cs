using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class DoubleConverter : IJsonConverter<double>
    {
        public const string Pattern = "0.0##########";

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
            builder.Write(value.ToString(Pattern, _cultureInfo));
        }

        public JsonData Write(double value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.Number);
        }

        object IJsonConverter.DeserializeObject(ref JsonTokenizer tokenizer) => Deserialize(ref tokenizer);
        
        object IJsonConverter.ReadObject(JsonData data) => Read(data);

        void IJsonConverter.SerializeObject(object value, TextWriter builder) => Serialize((double) value, builder);

        JsonData IJsonConverter.WriteObject(object value) => Write((double) value);
    }
}