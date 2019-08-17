namespace Velo.Serialization.Converters
{
    internal sealed class BoolConverter : IJsonConverter<bool>
    {
        public bool Convert(JToken token)
        {
            return token.TokenType != JTokenType.False;
        }
    }
}