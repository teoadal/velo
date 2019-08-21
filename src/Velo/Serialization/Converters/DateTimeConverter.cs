using System;
using System.Globalization;
using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class DateTimeConverter : IJsonConverter<DateTime>
    {
        private readonly CultureInfo _cultureInfo;

        public DateTimeConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public DateTime Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return DateTime.Parse(token.Value, _cultureInfo);
        }

        public void Serialize(DateTime value, StringBuilder builder)
        {
            builder.Append(value.ToString(_cultureInfo));
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((DateTime) value, builder);
    }
}