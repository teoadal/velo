using System.IO;
using Velo.Collections;
using Velo.Serialization.Models;
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

        public TElement[] Read(JsonData jsonData)
        {
            var arrayData = (JsonArray) jsonData;
            
            var array = new TElement[arrayData.Length];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = _elementConverter.Read(arrayData[i]);
            }
            
            return array;
        }

        public void Serialize(TElement[] array, TextWriter writer)
        {
            if (array == null)
            {
                writer.Write(JsonTokenizer.TokenNullValue);
                return;
            }

            writer.Write('[');

            for (var i = 0; i < array.Length; i++)
            {
                if (i > 0) writer.Write(',');
                _elementConverter.Serialize(array[i], writer);
            }

            writer.Write(']');
        }
        
        void IJsonConverter.Serialize(object value, TextWriter writer) => Serialize((TElement[]) value, writer);
    }
}