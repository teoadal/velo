namespace Velo.Serialization.Models
{
    public enum JsonDataType : byte
    {
        None = 0,
        Array,
        False,
        Null,
        Number,
        Object,
        String,
        True,
        Verbose
    }
}