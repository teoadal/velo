using System.Globalization;
using Velo.Serialization;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class SerializationInstaller
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