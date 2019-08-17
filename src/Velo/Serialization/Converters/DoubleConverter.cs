namespace Velo.Serialization.Converters
{
    internal sealed class DoubleConverter: IJsonConverter<double>
    {
        public double Convert(JToken token)
        {
            return double.Parse(token.Value);
        }
    }
}