using System.Text;

namespace Velo.Serialization.Converters
{
    internal interface IJsonConverter
    {
        void Serialize(object value, StringBuilder builder);
    }

    internal interface IJsonConverter<T> : IJsonConverter
    {
        T Deserialize(JsonTokenizer tokenizer);

        void Serialize(T value, StringBuilder builder);
    }
}