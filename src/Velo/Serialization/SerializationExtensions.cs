using System.IO;
using System.Text;
using Velo.Serialization.Converters;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization
{
    public static class SerializationExtensions
    {
        internal static T Read<T>(this IConvertersCollection converters, JsonData json)
        {
            return converters.Get<T>().Read(json);
        }

        internal static T Deserialize<T>(this IJsonConverter<T> converter, string json, StringBuilder? sb = null)
        {
            using var reader = new JsonReader(json);

            var tokenizer = new JsonTokenizer(reader, sb ?? new StringBuilder());

            if (converter.IsPrimitive) tokenizer.MoveNext();
            var result = converter.Deserialize(ref tokenizer);

            tokenizer.Dispose();

            return result;
        }

        public static string Serialize(this JsonData data)
        {
            var stringWriter = new StringWriter();
            data.Serialize(stringWriter);
            return stringWriter.ToString();
        }
    }
}