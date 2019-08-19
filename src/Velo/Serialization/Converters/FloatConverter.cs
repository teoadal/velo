using System.Globalization;

namespace Velo.Serialization.Converters
{
    internal sealed class FloatConverter: IJsonConverter<float>
    {
        private readonly CultureInfo _cultureInfo;

        public FloatConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public float Convert(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return float.Parse(token.Value, _cultureInfo);
        }
    }
}