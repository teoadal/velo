using System.Diagnostics;

namespace Velo.Serialization.Tokenization
{
    [DebuggerDisplay("{TokenType} '{Value}'")]
    internal readonly struct JsonToken
    {
        public static readonly JsonToken Empty = new JsonToken(JsonTokenType.None);

        public readonly JsonTokenType TokenType;

        public readonly string Value;

        public JsonToken(JsonTokenType tokenType, string value = null)
        {
            TokenType = tokenType;
            Value = value;
        }
    }
}