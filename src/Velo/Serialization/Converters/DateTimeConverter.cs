using System;
using System.Globalization;
using System.Text;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class DateTimeConverter : IJsonConverter<DateTime>
    {
        public bool IsPrimitive => true;
        
        private readonly CultureInfo _cultureInfo;

        public DateTimeConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public DateTime Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return DateTime.Parse(token.Value, _cultureInfo);
        }

        public DateTime Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return DateTime.Parse(jsonValue.Value, _cultureInfo);
        }
        
        public void Serialize(DateTime value, StringBuilder builder)
        {
            builder.Append('"');
            builder.Append(value.ToString("O", _cultureInfo));
            builder.Append('"');
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((DateTime) value, builder);
    }
}