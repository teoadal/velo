namespace Velo.Serialization.Converters
{
    internal sealed class StringConverter: IJsonConverter<string>
    {
        public string Convert(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return token.Value;
        }
    }
}