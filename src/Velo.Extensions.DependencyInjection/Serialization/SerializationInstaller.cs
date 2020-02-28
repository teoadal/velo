using System.Globalization;
using Velo.Serialization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializationInstaller
    {
        public static IServiceCollection AddJsonConverter(this IServiceCollection services, CultureInfo culture = null)
        {
            var convertersCollection = new ConvertersCollection(culture ?? CultureInfo.InvariantCulture);

            services
                .AddSingleton<IConvertersCollection>(convertersCollection)
                .AddSingleton(new JConverter(convertersCollection));

            return services;
        }
    }
}