using System;
using System.Linq;
using Velo.Extensions.DependencyInjection;
using Velo.Extensions.DependencyInjection.Logging;
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
        public static IServiceCollection AddLogs(this IServiceCollection services)
        {
            services
                .RemoveMicrosoftLogger()
                .AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(VeloLogger<>));

            services
                .AddSingleton<IRenderersCollection, RenderersCollection>()
                .AddSingleton(ProviderFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .EnsureJsonEnabled();

            return services;
        }

        public static IServiceCollection AddDefaultConsoleLogWriter(this IServiceCollection services,
            LogLevel level = LogLevel.Debug)
        {
            services.AddSingleton<ILogWriter>(new DefaultConsoleWriter(level));
            return services;
        }

        public static IServiceCollection AddDefaultLogEnrichers(this IServiceCollection services,
            string dateTimeFormat = "s")
        {
            services
                .AddSingleton<ILogEnricher>(new TimeStampEnricher(dateTimeFormat))
                .AddSingleton<ILogEnricher>(new LogLevelEnricher())
                .AddSingleton<ILogEnricher>(new SenderEnricher());

            return services;
        }

        public static IServiceCollection AddLogEnricher<TEnricher>(this IServiceCollection services)
            where TEnricher : ILogEnricher
        {
            var implementation = typeof(TEnricher);
            services.Add(new ServiceDescriptor(typeof(ILogEnricher), implementation, ServiceLifetime.Singleton));
            return services;
        }

        public static IServiceCollection AddLogWriter<TWriter>(this IServiceCollection services)
            where TWriter : ILogWriter
        {
            var implementation = typeof(TWriter);
            services.Add(new ServiceDescriptor(typeof(ILogWriter), implementation, ServiceLifetime.Singleton));
            return services;
        }

        private static ILogProvider ProviderFactory(IServiceProvider provider)
        {
            var writers = provider.GetArray<ILogWriter>();

            if (writers.Length == 0) return new NullLogProvider();

            var enrichers = provider.GetArray<ILogEnricher>();
            var renderers = provider.GetRequiredService<IRenderersCollection>();
            return new LogProvider(enrichers, renderers, writers);
        }

        private static IServiceCollection RemoveMicrosoftLogger(this IServiceCollection services)
        {
            var logger = services.FirstOrDefault(descriptor =>
                descriptor.ServiceType == typeof(Microsoft.Extensions.Logging.ILogger<>));
            if (logger != null) services.Remove(logger);

            return services;
        }
    }
}