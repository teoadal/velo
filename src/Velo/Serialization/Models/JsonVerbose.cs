using System.IO;

namespace Velo.Serialization.Models
{
    public sealed class JsonVerbose : JsonData
    {
        public readonly string Value;

        public JsonVerbose(string value) : base(JsonDataType.Verbose)
        {
            Value = value;
        }

        public override void Serialize(TextWriter writer)
        {
            writer.Write(Value);
        }
    }
}