using System;
using Microsoft.Extensions.DependencyInjection;
using Velo.Serialization;
using Velo.Settings.Provider;
using Velo.Settings.Sources;

namespace Velo.Extensions.DependencyInjection.Settings
{
    public static class SettingsInstaller
    {
        public static IServiceCollection AddSettings(this IServiceCollection services)
        {
            services
                .AddSingleton(ProviderFactory)
                .EnsureJsonEnabled();

            return services;
        }

        public static IServiceCollection AddJsonSettings(this IServiceCollection services,
            string path = "appsettings.json", bool required = false)
        {
            services.AddSingleton<ISettingsSource>(new JsonFileSource(path, required));

            return services;
        }

        public static IServiceCollection AddJsonSettings(this IServiceCollection services,
            Func<IServiceProvider, string> fileNameBuilder, bool required = false)
        {
            services.AddSingleton<ISettingsSource>(provider =>
                new JsonFileSource(fileNameBuilder(provider), required));

            return services;
        }

        private static ISettingsProvider ProviderFactory(IServiceProvider provider)
        {
            var sources = provider.GetArray<ISettingsSource>();

            if (sources.Length == 0) return new NullSettingsProvider();

            var convertersCollection = provider.GetRequiredService<IConvertersCollection>();
            return new SettingsProvider(sources, convertersCollection);
        }
    }
}