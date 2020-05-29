using System;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization
{
    public interface IJsonConverter
    {
        Type Contract { get; }
        
        bool IsPrimitive { get; }

        object DeserializeObject(JsonTokenizer tokenizer);

        object? ReadObject(JsonData jsonData);

        void SerializeObject(object value, TextWriter writer);

        JsonData WriteObject(object value);
    }

    public interface IJsonConverter<T> : IJsonConverter
    {
        T Deserialize(JsonTokenizer tokenizer);

        T Read(JsonData jsonData);

        void Serialize(T value, TextWriter output);

        JsonData Write(T value);
    }

    public abstract class JsonConverter<T> : IJsonConverter<T>
    {
        public Type Contract { get; }

        public bool IsPrimitive { get; }

        protected JsonConverter(bool isPrimitive)
        {
            Contract = Typeof<T>.Raw;
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