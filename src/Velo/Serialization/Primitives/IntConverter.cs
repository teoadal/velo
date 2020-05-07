using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
{
    internal sealed class IntConverter : JsonConverter<int>
    {
        public IntConverter() : base(true)
        {
        }

        public override int Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return int.Parse(token.Value);
        }

        public override int Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return int.Parse(jsonValue.Value);
        }

        public override void Serialize(int value, TextWriter output)
        {
            output.Write(value);
        }

        public override JsonData Write(int value)
        {
            return value == 0
                ? JsonValue.Zero
                : new JsonValue(value.ToString(), JsonDataType.Number);
        }
    }
}