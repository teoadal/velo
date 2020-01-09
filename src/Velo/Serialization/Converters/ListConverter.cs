using System.Collections.Generic;
using System.Text;
using Velo.Collections;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class ListConverter<TElement> : IJsonConverter<List<TElement>>
    {
        public bool IsPrimitive => false;

        private readonly IJsonConverter<TElement> _elementConverter;

        public ListConverter(IJsonConverter<TElement> elementConverter)
        {
            _elementConverter = elementConverter;
        }

        public List<TElement> Deserialize(ref JsonTokenizer tokenizer)
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
            
            var list = new List<TElement>(buffer.Length);

            foreach (var element in buffer)
            {
                list.Add(element);
            }

            return list;
        }

        public void Serialize(List<TElement> list, StringBuilder builder)
        {
            if (list == null)
            {
                builder.Append(JsonTokenizer.TokenNullValue);
                return;
            }

            builder.Append('[');

            var first = true;
            foreach (var element in list)
            {
                if (first) first = false;
                else builder.Append(',');

                _elementConverter.Serialize(element, builder);
            }

            builder.Append("]");
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) =>
            Serialize((List<TElement>) value, builder);
    }
}