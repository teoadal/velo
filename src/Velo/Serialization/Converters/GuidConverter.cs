using System;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class GuidConverter : JsonConverter<Guid>
    {
        public GuidConverter() : base(true)
        {
        }

        public override Guid Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return Guid.Parse(token.Value);
        }

        public override Guid Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return Guid.Parse(jsonValue.Value);
        }

        public override void Serialize(Guid value, TextWriter writer)
        {
            writer.Write('"');
            writer.Write(value.ToString());
            writer.Write('"');
        }

        public override JsonData Write(Guid value)
        {
            return new JsonValue(value.ToString(), JsonDataType.String);
        }
    }
}