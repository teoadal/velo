using System.Collections.Generic;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Collections
{
    internal sealed class ListConverter<TElement> : CollectionConverter<List<TElement>, TElement>
    {
        public ListConverter(IConvertersCollection converters) : base(converters)
        {
        }

        public override List<TElement> Deserialize(JsonTokenizer tokenizer)
        {
            // it's maybe different tokens
            if (tokenizer.Current.TokenType == JsonTokenType.None) tokenizer.MoveNext();
            if (tokenizer.Current.TokenType == JsonTokenType.Null) return null!;

            var buffer = GetBuffer();
            while (tokenizer.MoveNext())
            {
                var tokenType = tokenizer.Current.TokenType;

                if (tokenType == JsonTokenType.ArrayStart) continue;
                if (tokenType == JsonTokenType.ArrayEnd) break;

                var element = DeserializeElement(tokenizer);
                buffer.Add(element);
            }

            var list = new List<TElement>(buffer);

            ReturnBuffer(buffer);

            return list;
        }

        public override List<TElement> Read(JsonData jsonData)
        {
            if (jsonData.Type == JsonDataType.Null) return null!;

            var listData = (JsonArray) jsonData;
            var list = new List<TElement>(listData.Length);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var element in listData)
            {
                list.Add(ReadElement(element));
            }

            return list;
        }

        public override void Serialize(List<TElement>? list, TextWriter output)
        {
            if (list == null)
            {
                output.Write(JsonValue.NullToken);
                return;
            }

            output.Write('[');

            var first = true;
            foreach (var element in list)
            {
                if (first) first = false;
                else output.Write(',');

                SerializeElement(element, output);
            }

            output.Write(']');
        }

        public override JsonData Write(List<TElement>? list)
        {
            if (list == null) return JsonValue.Null;
            if (list.Count == 0) return JsonArray.Empty;

            var jsonElements = new JsonData[list.Count];

            for (var i = 0; i < jsonElements.Length; i++)
            {
                jsonElements[i] = WriteElement(list[i]);
            }

            return new JsonArray(jsonElements);
        }
    }
}