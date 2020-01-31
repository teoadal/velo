using System;
using System.Text;
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
        
        public void Serialize(Guid value, StringBuilder builder)
        {
            builder.Append('"');
            builder.Append(value.ToString());
            builder.Append('"');
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((Guid) value, builder);
    }
}