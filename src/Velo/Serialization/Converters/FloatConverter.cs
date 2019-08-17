namespace Velo.Serialization.Converters
{
    internal sealed class FloatConverter: IJsonConverter<float>
    {
        public float Convert(JToken token)
        {
            return float.Parse(token.Value);
        }
    }
}