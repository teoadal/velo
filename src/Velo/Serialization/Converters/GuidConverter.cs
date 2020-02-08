using System;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class GuidConverter : IJsonConverter<Guid>
    {
        public bool IsPrimitive => true;
        
        public Guid Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return Guid.Parse(token.Value);
        }

        public Guid Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return Guid.Parse(jsonValue.Value);
        }
        
        public void Serialize(Guid value, TextWriter writer)
        {
            writer.Write('"');
            writer.Write(value.ToString());
            writer.Write('"');
        }

        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((Guid) value, writer);
    }
}