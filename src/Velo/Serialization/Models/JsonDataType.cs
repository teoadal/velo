namespace Velo.Serialization.Models
{
    internal enum JsonDataType : byte
    {
        None = 0,
        Array,
        False,
        Null,
        Number,
        Object,
        String,
        True
    }
}