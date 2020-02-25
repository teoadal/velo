using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class FloatConverter : IJsonConverter<float>
    {
        public bool IsPrimitive => true;
        
        private readonly CultureInfo _cultureInfo;

        public FloatConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public float Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return float.Parse(token.Value, _cultureInfo);
        }

        public float Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return float.Parse(jsonValue.Value, _cultureInfo);
        }
        
        public void Serialize(float value, TextWriter writer)
        {
            writer.Write(value.ToString(_cultureInfo));
        }

        public JsonData Write(float value)
        {
            return new JsonValue(value.ToString(_cultureInfo), JsonDataType.Number);
        }

        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((float) value, writer);
    }
}