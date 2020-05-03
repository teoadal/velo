using System.IO;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.TestsModels.Serialization
{
    internal sealed class CustomConverter : JsonConverter<int>
    {
        public CustomConverter() : base(true)
        {
        }

        public override int Deserialize(JsonTokenizer tokenizer)
        {
            return 1;
        }

        public override int Read(JsonData jsonData)
        {
            return 1;
        }

        public override void Serialize(int value, TextWriter writer)
        {
            writer.Write("one");
        }

        public override JsonData Write(int value)
        {
            return JsonValue.String("one");
        }
    }
}