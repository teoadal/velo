using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Collections
{
    internal sealed class ArrayConverter<TElement> : CollectionConverter<TElement[], TElement>
    {
        public ArrayConverter(IConvertersCollection converters) : base(converters)
        {
        }

        public override TElement[] Deserialize(JsonTokenizer tokenizer)
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

            var result = buffer.ToArray();

            ReturnBuffer(buffer);

            return result;
        }

        public override TElement[] Read(JsonData jsonData)
        {
            if (jsonData.Type == JsonDataType.Null) return null!;

            var arrayData = (JsonArray) jsonData;

            var array = new TElement[arrayData.Length];
            for (var i = array.Length - 1; i >= 0; i--)
            {
                array[i] = ReadElement(arrayData[i]);
            }

            return array;
        }

        public override void Serialize(TElement[]? array, TextWriter output)
        {
            if (array == null)
            {
                output.Write(JsonValue.NullToken);
                return;
            }

            output.WriteArrayStart();

            for (var i = 0; i < array.Length; i++)
            {
                if (i > 0) output.Write(',');
                SerializeElement(array[i], output);
            }

            output.WriteArrayEnd();
        }

        public override JsonData Write(TElement[]? array)
        {
            if (array == null) return JsonValue.Null;
            if (array.Length == 0) return JsonArray.Empty;

            var jsonElements = new JsonData[array.Length];

            for (var i = array.Length - 1; i >= 0; i--)
            {
                jsonElements[i] = WriteElement(array[i]);
            }

            return new JsonArray(jsonElements);
        }
    }
}