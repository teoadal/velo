using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Objects
{
    internal sealed class PropertyConverter<TOwner> : IPropertyConverter<TOwner>
    {
        private readonly Action<JsonTokenizer, TOwner> _deserialize;
        private readonly Action<JsonObject, TOwner> _read;
        private readonly Action<TOwner, TextWriter> _serialize;
        private readonly Action<TOwner, JsonObject> _write;

        public PropertyConverter(PropertyInfo property, IJsonConverter valueConverter)
        {
            var instance = Expression.Parameter(Typeof<TOwner>.Raw, "instance");
            var converter = Expression.Constant(valueConverter, valueConverter.GetType());

            _serialize = SerializationUtils.BuildSerialize<Action<TOwner, TextWriter>>(instance, property, converter);
            _write = SerializationUtils.BuildWrite<Action<TOwner, JsonObject>>(instance, property, converter);

            if (property.CanWrite)
            {
                _deserialize = SerializationUtils.BuildDeserialize<Action<JsonTokenizer, TOwner>>(instance, property, converter);
                _read = SerializationUtils.BuildRead<Action<JsonObject, TOwner>>(instance, property, converter);
            }
            else
            {
                _deserialize = ReadonlyProperty;
                _read = ReadonlyProperty;
            }
        }

        public void Deserialize(JsonTokenizer source, TOwner instance) => _deserialize(source, instance);

        public void Read(JsonObject source, TOwner instance) => _read(source, instance);

        public void Serialize(TOwner instance, TextWriter output) => _serialize(instance, output);

        public void Write(TOwner instance, JsonObject output) => _write(instance, output);
        
        private static void ReadonlyProperty(JsonTokenizer tokenizer, TOwner obj)
        {
        }

        private static void ReadonlyProperty(JsonData json, TOwner obj)
        {
        }
    }
}