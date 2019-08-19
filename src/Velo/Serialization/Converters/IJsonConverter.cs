namespace Velo.Serialization.Converters
{
    internal interface IJsonConverter
    {
    }

    internal interface IJsonConverter<out T> : IJsonConverter
    {
        T Convert(JsonTokenizer tokenizer);
    }
}