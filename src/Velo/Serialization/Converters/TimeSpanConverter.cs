using System;
using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class TimeSpanConverter : IJsonConverter<TimeSpan>
    {
        public bool IsPrimitive => true;
        
        private readonly CultureInfo _cultureInfo;

        public TimeSpanConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public TimeSpan Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return TimeSpan.Parse(token.Value, _cultureInfo);
        }

        public TimeSpan Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return TimeSpan.Parse(jsonValue.Value, _cultureInfo);
        }
        
        public void Serialize(TimeSpan value, TextWriter writer)
        {
            writer.Write('"');
            writer.Write(value.ToString("g", _cultureInfo));
            writer.Write('"');
        }

        public JsonData Write(TimeSpan value)
        {
            return new JsonValue(value.ToString("g", _cultureInfo), JsonDataType.String);
        }

        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((TimeSpan) value, writer);   
    }
}