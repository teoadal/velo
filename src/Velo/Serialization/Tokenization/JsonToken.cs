using System.Diagnostics;

namespace Velo.Serialization.Tokenization
{
    [DebuggerDisplay("{" + nameof(TokenType) + "}")]
    public readonly struct JsonToken
    {
        public readonly JsonTokenType TokenType;

        public readonly string? Value;

        internal JsonToken(JsonTokenType tokenType, string? value = null)
        {
            TokenType = tokenType;
            Value = value;
        }
    }
}