namespace Velo.Serialization.Converters
{
    internal sealed class IntConverter: IJsonConverter<int>
    {
        public int Convert(JToken token)
        {
            return int.Parse(token.Value);
        }
    }
}