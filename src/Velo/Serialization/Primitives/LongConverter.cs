using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
{
    internal sealed class LongConverter : JsonConverter<long>
    {
        public LongConverter() : base(true)
        {
        }

        public override long Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return long.Parse(token.Value);
        }

        public override long Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return long.Parse(jsonValue.Value);
        }

        public override void Serialize(long value, TextWriter output)
        {
            output.Write(value);
        }

        public override JsonData Write(long value)
        {
            return value == 0L
                ? JsonValue.Zero
                : new JsonValue(value.ToString(), JsonDataType.Number);
        }
    }
}