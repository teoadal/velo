using System;
using System.Globalization;
using System.IO;
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

        public void Serialize(DateTime value, TextWriter writer)
        {
            writer.Write('"');
            writer.Write(value.ToString("O", _cultureInfo));
            writer.Write('"');
        }

        public JsonData Write(DateTime value)
        {
            return new JsonValue(value.ToString("O", _cultureInfo), JsonDataType.String);
        }

        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((DateTime) value, writer);
    }
}