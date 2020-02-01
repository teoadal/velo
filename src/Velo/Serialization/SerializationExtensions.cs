using System.Globalization;
using Velo.DependencyInjection;

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
    }
}