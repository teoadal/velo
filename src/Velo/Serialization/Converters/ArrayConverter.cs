using System;
using System.Collections.Generic;
using System.Text;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class ArrayConverter<TElement> : IJsonConverter<TElement[]>
    {
        public bool IsPrimitive => false;
        
        [ThreadStatic] private static List<TElement> _buffer;
        
        private readonly IJsonConverter<TElement> _elementConverter;

        public ArrayConverter(IJsonConverter<TElement> elementConverter)
        {
            _elementConverter = elementConverter;
        }

        public TElement[] Deserialize(JsonTokenizer tokenizer)
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

            var array = new TElement[_buffer.Count];
            for (var i = 0; i < array.Length; i++)
                array[i] = _buffer[i];

            _buffer.Clear();

            return array;
        }

        public void Serialize(TElement[] array, StringBuilder builder)
        {
            if (array == null)
            {
                builder.Append(JsonTokenizer.TOKEN_NULL_VALUE);
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