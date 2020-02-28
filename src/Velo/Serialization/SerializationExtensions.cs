using System.Globalization;
using Velo.DependencyInjection;
using Velo.Serialization.Models;

namespace Velo.Serialization
{
    public static class SerializationExtensions
    {
        public static DependencyCollection AddJsonConverter(this DependencyCollection collection,
            CultureInfo culture = null)
        {
            var convertersCollection = new ConvertersCollection(culture ?? CultureInfo.InvariantCulture);

            collection
                .AddInstance<IConvertersCollection>(convertersCollection)
                .AddInstance(new JConverter(convertersCollection));

            return collection;
        }

        internal static T Read<T>(this ConvertersCollection converters, JsonData json)
        {
            return converters.Get<T>().Read(json);
        }
    }
}