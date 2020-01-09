using System.Text;
using Velo.Collections;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class ArrayConverter<TElement> : IJsonConverter<TElement[]>
    {
        public bool IsPrimitive => false;
        
        private readonly IJsonConverter<TElement> _elementConverter;

        public ArrayConverter(IJsonConverter<TElement> elementConverter)
        {
            _elementConverter = elementConverter;
        }

        public TElement[] Deserialize(ref JsonTokenizer tokenizer)
        {
            var buffer = new LocalList<TElement>();
            
            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;
                var tokenType = token.TokenType;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (tokenType == JsonTokenType.Null) return null;
                if (tokenType == JsonTokenType.ArrayStart) continue;
                if (tokenType == JsonTokenType.ArrayEnd) break;

                var element = _elementConverter.Deserialize(ref tokenizer);
                buffer.Add(element);
            }

            return buffer.ToArray();
        }

        public void Serialize(TElement[] array, StringBuilder builder)
        {
            if (array == null)
            {
                builder.Append(JsonTokenizer.TokenNullValue);
                return;
            }

            builder.Append('[');

            for (var i = 0; i < array.Length; i++)
            {
                if (i > 0) builder.Append(',');
                _elementConverter.Serialize(array[i], builder);
            }

            builder.Append("]");
        }
        
        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((TElement[]) value, builder);
    }
}