using System.Collections.Generic;
using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class ArrayConverter<T> : IJsonConverter<T[]>
    {
        private readonly List<T> _buffer;
        private readonly IJsonConverter<T> _elementConverter;

        public ArrayConverter(IJsonConverter<T> elementConverter)
        {
            _elementConverter = elementConverter;

            _buffer = new List<T>(10);
        }

        public T[] Deserialize(JsonTokenizer tokenizer)
        {
            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                var tokenType = current.TokenType;

                if (tokenType == JsonTokenType.ArrayStart) continue;
                if (tokenType == JsonTokenType.ArrayEnd) break;

                var element = _elementConverter.Deserialize(tokenizer);
                _buffer.Add(element);
            }

            var array = new T[_buffer.Count];
            for (var i = 0; i < array.Length; i++)
                array[i] = _buffer[i];

            _buffer.Clear();

            return array;
        }

        public void Serialize(T[] array, StringBuilder builder)
        {
            if (array == null)
            {
                builder.Append(JsonTokenizer.NullValue);
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

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((T[]) value, builder);
    }
}