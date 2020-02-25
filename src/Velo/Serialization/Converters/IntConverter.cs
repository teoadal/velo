using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class IntConverter : IJsonConverter<int>
    {
        public bool IsPrimitive => true;
        
        public int Deserialize(ref JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return int.Parse(token.Value);
        }

        public int Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return int.Parse(jsonValue.Value);
        }
        
        public void Serialize(int value, TextWriter writer)
        {
            writer.Write(value);
        }

        public JsonData Write(int value)
        {
            return value == 0 
                ? JsonValue.Zero 
                : new JsonValue(value.ToString(), JsonDataType.Number);
        }

        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((int) value, writer);
    }
}