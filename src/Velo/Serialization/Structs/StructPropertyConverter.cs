using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Structs
{
    internal readonly struct StructPropertyConverter<TOwner>
        where TOwner : struct
    {
        public readonly DeserializeDelegate Deserialize;
        public readonly ReadDelegate Read;
        public readonly SerializeDelegate Serialize;
        public readonly WriteDelegate Write;

        public StructPropertyConverter(Type owner, PropertyInfo property, IJsonConverter valueConverter)
        {
            var instance = Expression.Parameter(owner.MakeByRefType());
            var converter = Expression.Constant(valueConverter, valueConverter.GetType());

            Serialize = SerializationUtils.BuildSerialize<SerializeDelegate>(instance, property, converter);
            Write = SerializationUtils.BuildWrite<WriteDelegate>(instance, property, converter);

            if (property.CanWrite)
            {
                Deserialize = SerializationUtils.BuildDeserialize<DeserializeDelegate>(instance, property, converter);
                Read = SerializationUtils.BuildRead<ReadDelegate>(instance, property, converter);
            }
            else
            {
                Deserialize = ReadonlyProperty;
                Read = ReadonlyProperty;
            }
        }

        private static void ReadonlyProperty(JsonTokenizer tokenizer, ref TOwner instance)
        {
        }

        private static void ReadonlyProperty(JsonData json, ref TOwner instance)
        {
        }

        #region Delegates

        public delegate void DeserializeDelegate(JsonTokenizer source, ref TOwner instance);

        public delegate void ReadDelegate(JsonObject source, ref TOwner instance);

        public delegate void SerializeDelegate(ref TOwner owner, TextWriter output);

        public delegate void WriteDelegate(ref TOwner owner, JsonObject output);

        #endregion
    }
}