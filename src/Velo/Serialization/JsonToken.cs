namespace Velo.Serialization
{
    internal readonly struct JsonToken
    {
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

    public enum JsonTokenType : byte
    {
        None = 0,
        ArrayStart,
        ArrayEnd,
        False,
        Number,
        Null,
        ObjectStart,
        ObjectEnd,
        Property,
        String,
        True,
    }
}