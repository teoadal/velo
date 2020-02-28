using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Renderers;
using Velo.Logging.Writers;
using Velo.Utils;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class LoggingInstaller
    {
        public static DependencyCollection AddLogging(this DependencyCollection collection)
        {
            collection
                .AddSingleton<IRendererCollection, RendererCollection>()
                .AddFactory(new LogProviderFactory())
                .AddScoped(typeof(ILogger<>), typeof(Logger<>));

            return collection;
        }

        public static DependencyCollection AddDefaultConsoleLogWriter(this DependencyCollection collection,
            LogLevel level = LogLevel.Debug)
        {
            collection.AddInstance(new DefaultConsoleLogWriter(level));
            return collection;
        }

        public static DependencyCollection AddDefaultLogEnrichers(this DependencyCollection collection)
        {
            AddLogEnricher<LogLevelEnricher>(collection);
            AddLogEnricher<SenderEnricher>(collection);
            AddLogEnricher<TimeStampEnricher>(collection);

            return collection;
        }

        public static DependencyCollection AddLogEnricher<TEnricher>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TEnricher : ILogEnricher
        {
            var implementation = Typeof<TEnricher>.Raw;
            var contracts = new[] {Typeof<ILogEnricher>.Raw, implementation};

            collection.AddDependency(contracts, implementation, lifetime);

            return collection;
        }

        public static DependencyCollection AddLogEnricher(this DependencyCollection collection,
            ILogEnricher logEnricher)
        {
            collection.AddInstance(logEnricher);
            return collection;
        }

        public static DependencyCollection AddLogWriter<TWriter>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TWriter : ILogWriter
        {
            var implementation = Typeof<TWriter>.Raw;
            var contracts = new[] {Typeof<ILogWriter>.Raw, implementation};

            collection.AddDependency(contracts, implementation, lifetime);

            return collection;
        }

        public static DependencyCollection AddLogWriter(this DependencyCollection collection, ILogWriter logWriter)
        {
            collection.AddInstance(logWriter);
            return collection;
        }
    }
}