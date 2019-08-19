using System.Globalization;
using System.Text;

namespace Velo.Serialization.Converters
{
    internal sealed class DoubleConverter : IJsonConverter<double>
    {
        private readonly CultureInfo _cultureInfo;

        public DoubleConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public double Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return double.Parse(token.Value, _cultureInfo);
        }

        public void Serialize(double value, StringBuilder builder)
        {
            builder.Append(value.ToString(_cultureInfo));
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((double) value, builder);
    }
}