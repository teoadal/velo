using System;
using System.Globalization;
using System.IO;
using Velo.Serialization.Models;
using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Primitives
{
    internal sealed class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public const string Pattern = "c";

        private readonly CultureInfo _cultureInfo;

        public TimeSpanConverter(CultureInfo cultureInfo) : base(true)
        {
            _cultureInfo = cultureInfo;
        }

        public override TimeSpan Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;
            return TimeSpan.Parse(token.Value, _cultureInfo);
        }

        public override TimeSpan Read(JsonData jsonData)
        {
            var jsonValue = (JsonValue) jsonData;
            return TimeSpan.Parse(jsonValue.Value, _cultureInfo);
        }

        public override void Serialize(TimeSpan value, TextWriter output)
        {
            output.WriteString(value.ToString(Pattern, _cultureInfo));
        }

        public override JsonData Write(TimeSpan value)
        {
            return new JsonValue(value.ToString(Pattern, _cultureInfo), JsonDataType.String);
        }
    }
}