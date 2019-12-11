using System.Diagnostics;

namespace Velo.Serialization.Tokenization
{
    [DebuggerDisplay("{TokenType} '{Value}'")]
    internal readonly ref struct JsonToken
    {
        public readonly JsonTokenType TokenType;

        public readonly string Value;

        public JsonToken(JsonTokenType tokenType, string value = null)
        {
            TokenType = tokenType;
            Value = value;
        }
    }
}