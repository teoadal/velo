using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class IntConverter : IJsonConverter<int>
    {
        public bool IsPrimitive => true;
        
        public int Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return int.Parse(token.Value);
        }

        public void Serialize(int value, StringBuilder builder)
        {
            builder.Append(value);
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((int) value, builder);
    }
}