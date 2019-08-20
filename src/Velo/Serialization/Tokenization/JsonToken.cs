namespace Velo.Serialization.Tokenization
{
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

        public bool IsEmpty() => TokenType == JsonTokenType.None;

        public override string ToString()
        {
            return string.IsNullOrEmpty(Value)
                ? TokenType.ToString()
                : $"{TokenType} '{Value}'";
        }
    }
}