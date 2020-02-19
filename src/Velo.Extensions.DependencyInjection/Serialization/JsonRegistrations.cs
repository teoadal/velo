using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Velo.Serialization;

namespace Velo.Extensions.DependencyInjection.Serialization
{
    public static class JsonRegistrations
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