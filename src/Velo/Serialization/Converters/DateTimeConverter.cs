using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;
using Velo.Utils;

namespace Velo.Serialization.Converters
{
    internal sealed class DateTimeConverter : JsonConverter<DateTime>
    {
        public const string Pattern = "yyyy-MM-ddTHH:mm:ss.FFFFFFFK";

        private readonly CultureInfo _cultureInfo;

        public DateTimeConverter(CultureInfo cultureInfo) : base(true)
        {
            _cultureInfo = cultureInfo;
        }

        public override DateTime Deserialize(JsonTokenizer tokenizer)
        {
            var value = tokenizer.Current.Value;
            return Parse(value ?? throw Error.Null("Null isn't convert to DateTime"));
        }

        public override DateTime Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return Parse(jsonValue.Value);
        }

        public override void Serialize(DateTime value, TextWriter writer)
        {
            writer.Write('"');
            writer.Write(value.ToString(Pattern, _cultureInfo));
            writer.Write('"');
        }

        public override JsonData Write(DateTime value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.String);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime Parse(string value)
        {
            return value[value.Length - 1] == 'Z'
                ? DateTimeOffset.Parse(value, _cultureInfo).DateTime
                : DateTime.Parse(value, _cultureInfo);
        }
    }
}