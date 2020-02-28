using Velo.Serialization.Models;

namespace Velo.Serialization
{
    public static class SerializationExtensions
    {
        internal static T Read<T>(this ConvertersCollection converters, JsonData json)
        {
            return converters.Get<T>().Read(json);
        }
    }
}