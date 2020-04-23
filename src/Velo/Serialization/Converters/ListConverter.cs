using System.Collections.Generic;
using System.IO;
using Velo.Collections;
using Velo.Serialization.Models;
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

        public List<TElement> Read(JsonData jsonData)
        {
            if (jsonData.Type == JsonDataType.Null) return null;

            var listData = (JsonArray) jsonData;
            var list = new List<TElement>(listData.Length);
            
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var value in listData)
            {
                list.Add(_elementConverter.Read(value));
            }

            return list;
        }

        public void Serialize(List<TElement> list, TextWriter writer)
        {
            if (list == null)
            {
                writer.Write(JsonTokenizer.TokenNullValue);
                return;
            }

            writer.Write('[');

            var first = true;
            foreach (var element in list)
            {
                if (first) first = false;
                else writer.Write(',');

                _elementConverter.Serialize(element, writer);
            }

            writer.Write(']');
        }

        public JsonData Write(List<TElement> list)
        {
            if (list == null) return JsonValue.Null;
            if (list.Count == 0) return JsonArray.Empty;

            var jsonElements = new JsonData[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                jsonElements[i] = _elementConverter.Write(list[i]);
            }

            return new JsonArray(jsonElements);
        }

        object IJsonConverter.ReadObject(JsonData data) => Read(data);

        void IJsonConverter.SerializeObject(object value, TextWriter writer) =>
            Serialize((List<TElement>) value, writer);

        JsonData IJsonConverter.WriteObject(object value) => Write((List<TElement>) value);
    }
}