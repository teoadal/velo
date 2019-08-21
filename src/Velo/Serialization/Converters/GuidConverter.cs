using System;
using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class GuidConverter : IJsonConverter<Guid>
    {
        public Guid Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return Guid.Parse(token.Value);
        }

        public void Serialize(Guid value, StringBuilder builder)
        {
            builder.Append(value.ToString());
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((Guid) value, builder);
    }
}