using System;
using Velo.Extensions.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Renderers;
using Velo.Logging.Writers;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoggingInstaller
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services
                .AddSingleton<IRenderersCollection, RenderersCollection>()
                .AddScoped(ProviderFactory)
                .AddScoped(typeof(ILogger<>), typeof(Logger<>));

            return services;
        }

        public static IServiceCollection AddDefaultConsoleLogWriter(this IServiceCollection services,
            LogLevel level = LogLevel.Debug)
        {
            services.AddSingleton<ILogWriter>(new DefaultConsoleWriter(level));
            return services;
        }

        public static IServiceCollection AddDefaultLogEnrichers(this IServiceCollection services)
        {
            services
                .AddSingleton<ILogEnricher, LogLevelEnricher>()
                .AddSingleton<ILogEnricher, SenderEnricher>()
                .AddSingleton<ILogEnricher, TimeStampEnricher>();

            return services;
        }

        public static IServiceCollection AddLogEnricher<TEnricher>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TEnricher : ILogEnricher
        {
            var implementation = typeof(TEnricher);
            services.Add(new ServiceDescriptor(typeof(ILogEnricher), implementation, lifetime));
            return services;
        }

        public static IServiceCollection AddLogWriter<TWriter>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TWriter : ILogWriter
        {
            var implementation = typeof(TWriter);
            services.Add(new ServiceDescriptor(typeof(ILogWriter), implementation, lifetime));
            return services;
        }

        private static ILogProvider ProviderFactory(IServiceProvider provider)
        {
            var writers = provider.GetArray<ILogWriter>();

            if (writers.Length == 0) return NullLogProvider.Instance;

            var enrichers = provider.GetArray<ILogEnricher>();
            var renderers = provider.GetRequiredService<IRenderersCollection>();
            return new LogProvider(enrichers, renderers, writers);
        }
    }
}