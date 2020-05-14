using System.Globalization;
using Velo.Serialization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class SerializationInstaller
    {
        public static IServiceCollection AddJsonConverter(this IServiceCollection services, CultureInfo culture = null)
        {
            services
                .AddSingleton<IConvertersCollection>(provider => new ConvertersCollection(provider, culture))
                .AddSingleton(provider => new JConverter(provider.GetService<IConvertersCollection>()));

            return services;
        }

        internal static IServiceCollection EnsureJsonEnabled(this IServiceCollection services)
        {
            if (!services.Contains(typeof(IConvertersCollection)))
            {
                AddJsonConverter(services);
            }

            return services;
        }
    }
}