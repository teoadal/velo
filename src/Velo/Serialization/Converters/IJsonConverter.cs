using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal interface IJsonConverter
    {
        bool IsPrimitive { get; }

        object DeserializeObject(ref JsonTokenizer tokenizer);
        
        object ReadObject(JsonData jsonData);

        void SerializeObject(object value, TextWriter writer);

        JsonData WriteObject(object value);
    }

    internal interface IJsonConverter<T> : IJsonConverter
    {
        T Deserialize(ref JsonTokenizer tokenizer);

        T Read(JsonData jsonData);

        void Serialize(T value, TextWriter writer);

        JsonData Write(T value);
    }
}