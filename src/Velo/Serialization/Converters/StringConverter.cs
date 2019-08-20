using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class StringConverter : IJsonConverter<string>
    {
        public string Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return token.Value;
        }

        public void Serialize(string value, StringBuilder builder)
        {
            builder.Append('"');
            builder.Append(value);
            builder.Append('"');
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((string) value, builder);
    }
}