using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class FloatConverter : IJsonConverter<float>
    {
        public const string Pattern = "0.0##########";
        
        public bool IsPrimitive => true;

        private readonly CultureInfo _cultureInfo;

        public FloatConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public float Deserialize(JsonTokenizer tokenizer)
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
            writer.Write(value.ToString(Pattern, _cultureInfo));
        }

        public JsonData Write(float value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.Number);
        }

        object IJsonConverter.DeserializeObject(JsonTokenizer tokenizer) => Deserialize(tokenizer);
        
        object IJsonConverter.ReadObject(JsonData data) => Read(data);

        void IJsonConverter.SerializeObject(object value, TextWriter writer) => Serialize((float) value, writer);

        JsonData IJsonConverter.WriteObject(object value) => Write((float) value);
    }
}