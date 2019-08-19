namespace Velo.Serialization.Converters
{
    internal sealed class IntConverter: IJsonConverter<int>
    {
        public int Convert(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return int.Parse(token.Value);
        }
    }
}