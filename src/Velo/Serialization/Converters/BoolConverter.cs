using System;

namespace Velo.Serialization.Converters
{
    internal sealed class BoolConverter : IJsonConverter<bool>
    {
        public bool Convert(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;

            switch (token.TokenType)
            {
                case JTokenType.True:
                    return true;
                case JTokenType.False:
                    return false;
                default:
                    throw new InvalidOperationException($"Invalid boolean token '{token}'");
            }
        }
    }
}