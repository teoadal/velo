using System.Collections.Generic;

namespace Velo.Serialization.Converters
{
    internal sealed class ArrayConverter<T>: IJsonConverter<T[]>
    {
        private readonly List<T> _buffer;
        private readonly IJsonConverter<T> _elementConverter;

        public ArrayConverter(IJsonConverter<T> elementConverter)
        {
            _elementConverter = elementConverter;
            
            _buffer = new List<T>(10);
        }

        public T[] Convert(JsonTokenizer tokenizer)
        {
            while (tokenizer.MoveNext())
            {
                var current = tokenizer.Current;
                var tokenType = current.TokenType;
                
                if (tokenType == JTokenType.ArrayStart) continue;
                if (tokenType == JTokenType.ArrayEnd) break;

                var element = _elementConverter.Convert(tokenizer);
                _buffer.Add(element);
            }

            var array = new T[_buffer.Count];
            for (var i = 0; i < array.Length; i++)
                array[i] = _buffer[i];
            
            _buffer.Clear();
            
            return array;
        }
    }
}