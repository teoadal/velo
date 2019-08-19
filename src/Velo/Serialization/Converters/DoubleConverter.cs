using System.Globalization;

namespace Velo.Serialization.Converters
{
    internal sealed class DoubleConverter: IJsonConverter<double>
    {
        private readonly CultureInfo _cultureInfo;

        public DoubleConverter(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }
        
        public double Convert(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return double.Parse(token.Value, _cultureInfo);
        }
    }
}