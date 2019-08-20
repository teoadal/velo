using System.Globalization;
using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class FloatConverter : IJsonConverter<float>
    {
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

        public void Serialize(float value, StringBuilder builder)
        {
            builder.Append(value.ToString(_cultureInfo));
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((float) value, builder);
    }
}