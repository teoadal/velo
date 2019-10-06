using System;
using System.Collections.Generic;
using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class ListConverter<TElement> : IJsonConverter<List<TElement>>
    {
        public bool IsPrimitive => false;
        
        [ThreadStatic] private static List<TElement> _buffer;
        
        private readonly IJsonConverter<TElement> _elementConverter;

        public ListConverter(IJsonConverter<TElement> elementConverter)
        {
            _elementConverter = elementConverter;
        }

        public List<TElement> Deserialize(JsonTokenizer tokenizer)
        {
            if (_buffer == null) _buffer = new List<TElement>(10);

            while (tokenizer.MoveNext())
            {
                var token = tokenizer.Current;
                var tokenType = token.TokenType;

                if (tokenType == JsonTokenType.ArrayStart) continue;
                if (tokenType == JsonTokenType.ArrayEnd) break;

                var element = _elementConverter.Deserialize(tokenizer);
                _buffer.Add(element);
            }

            var list = new List<TElement>(_buffer.Count);
            list.AddRange(_buffer);
            
            _buffer.Clear();
            
            return list;
        }

        public void Serialize(List<TElement> list, StringBuilder builder)
        {
            if (list == null)
            {
                builder.Append(JsonTokenizer.TOKEN_NULL_VALUE);
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