namespace Velo.Serialization
{
    internal readonly struct JToken
    {
        public readonly JTokenType TokenType;

        public readonly string Value;

        public JToken(JTokenType tokenType, string value = null)
        {
            TokenType = tokenType;
            Value = value;
        }

        public bool IsEmpty() => TokenType == JTokenType.None;

        public override string ToString()
        {
            return string.IsNullOrEmpty(Value)
                ? TokenType.ToString()
                : $"{TokenType} '{Value}'";
        }
    }

    public enum JTokenType : byte
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