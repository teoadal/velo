using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal interface IJsonConverter
    {
        bool IsPrimitive { get; }
        
        void Serialize(object value, StringBuilder builder);
    }

    internal interface IJsonConverter<T> : IJsonConverter
    {
        T Deserialize(ref JsonTokenizer tokenizer);

        void Serialize(T value, StringBuilder builder);
    }
}