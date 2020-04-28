using System.Globalization;
using Velo.Serialization;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class SerializationInstaller
    {
        public static DependencyCollection AddJsonConverter(this DependencyCollection dependencies,
            CultureInfo? culture = null)
        {
            var convertersCollection = new ConvertersCollection(culture ?? CultureInfo.InvariantCulture);

            dependencies
                .AddInstance<IConvertersCollection>(convertersCollection)
                .AddInstance(new JConverter(convertersCollection));

            return dependencies;
        }

        internal static DependencyCollection EnsureJsonEnabled(this DependencyCollection dependencies)
        {
            if (!dependencies.Contains(typeof(IConvertersCollection)))
            {
                AddJsonConverter(dependencies);
            }

            return dependencies;
        }
    }
}