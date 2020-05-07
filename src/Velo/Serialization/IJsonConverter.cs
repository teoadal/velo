using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization
{
    internal interface IJsonConverter
    {
        bool IsPrimitive { get; }

        object DeserializeObject(JsonTokenizer tokenizer);

        object? ReadObject(JsonData jsonData);

        void SerializeObject(object value, TextWriter writer);

        JsonData WriteObject(object value);
    }

    internal interface IJsonConverter<T> : IJsonConverter
    {
        T Deserialize(JsonTokenizer tokenizer);

        T Read(JsonData jsonData);

        void Serialize(T value, TextWriter output);

        JsonData Write(T value);
    }

    internal abstract class JsonConverter<T> : IJsonConverter<T>
    {
        public bool IsPrimitive { get; }

        protected JsonConverter(bool isPrimitive)
        {
            IsPrimitive = isPrimitive;
        }

        public abstract T Deserialize(JsonTokenizer tokenizer);

        public abstract T Read(JsonData jsonData);

        public abstract void Serialize(T value, TextWriter output);

        public abstract JsonData Write(T value);

        object IJsonConverter.DeserializeObject(JsonTokenizer tokenizer) => Deserialize(tokenizer)!;
        object IJsonConverter.ReadObject(JsonData jsonData) => Read(jsonData)!;
        void IJsonConverter.SerializeObject(object value, TextWriter output) => Serialize((T) value, output);
        JsonData IJsonConverter.WriteObject(object value) => Write((T) value);
    }
}