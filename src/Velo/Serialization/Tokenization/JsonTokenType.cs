namespace Velo.Serialization.Tokenization
{
    internal enum JsonTokenType : byte
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
        True
    }
}