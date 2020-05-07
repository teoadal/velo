using System;
using System.Collections.Generic;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Collections
{
    internal abstract class CollectionConverter<TCollection, TElement> : IJsonConverter<TCollection>
    {
        [ThreadStatic] 
        private static List<TElement>? _buffer;

        public bool IsPrimitive => false;

        private readonly IConvertersCollection _converters;
        private readonly IJsonConverter<TElement> _elementConverter;
        private readonly Type _elementType;
        private readonly Func<TElement, bool> _isNull;

        protected CollectionConverter(IConvertersCollection converters)
        {
            _converters = converters;
            _elementConverter = converters.Get<TElement>();
            _elementType = Typeof<TElement>.Raw;
            _isNull = _elementType.IsClass
                ? new Func<TElement, bool>(reference => reference == null)
                : notReference => false;
        }

        public abstract TCollection Deserialize(JsonTokenizer tokenizer);

        public abstract TCollection Read(JsonData jsonData);

        public abstract void Serialize(TCollection value, TextWriter output);

        public abstract JsonData Write(TCollection value);

        protected TElement DeserializeElement(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            var tokenType = token.TokenType;

            return tokenType != JsonTokenType.Null
                ? _elementConverter.Deserialize(tokenizer)
                : default!;
        }

        protected TElement ReadElement(JsonData element)
        {
            return element.Type != JsonDataType.Null
                ? _elementConverter.Read(element)
                : default!;
        }

        protected void SerializeElement(TElement element, TextWriter output)
        {
            if (_isNull(element))
            {
                output.Write(JsonValue.NullToken);
                return;
            }

            var elementType = element!.GetType();

            if (elementType == _elementType)
            {
                _elementConverter.Serialize(element, output);
            }
            else
            {
                var converter = _converters.Get(elementType);
                converter.SerializeObject(element, output);
            }
        }

        protected JsonData WriteElement(TElement element)
        {
            if (_isNull(element)) return JsonValue.Null;

            var elementType = element!.GetType();

            return elementType == _elementType
                ? _elementConverter.Write(element)
                : _converters.Get(elementType).WriteObject(element);
        }

        protected List<TElement> GetBuffer()
        {
            if (_buffer == null) return new List<TElement>(); 
            var result = _buffer;
            _buffer = null;
            return result;
        }

        protected void ReturnBuffer(List<TElement> buffer)
        {
            buffer.Clear();
            _buffer = buffer;
        }

        object IJsonConverter.DeserializeObject(JsonTokenizer tokenizer) => Deserialize(tokenizer)!;
        object IJsonConverter.ReadObject(JsonData jsonData) => Read(jsonData)!;
        void IJsonConverter.SerializeObject(object value, TextWriter writer) => Serialize((TCollection) value, writer);
        JsonData IJsonConverter.WriteObject(object value) => Write((TCollection) value);
    }
}