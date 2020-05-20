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
            dependencies
                .AddSingleton<IConvertersCollection>(provider => new ConvertersCollection(provider, culture))
                .AddSingleton<JConverter>();

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