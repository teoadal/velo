namespace Velo.Serialization.Converters
{
    internal sealed class StringConverter: IJsonConverter<string>
    {
        public string Convert(JToken token)
        {
            return token.Value;
        }
    }
}