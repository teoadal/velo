using Velo.DependencyInjection;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Renderers;
using Velo.Logging.Writers;
using Velo.Utils;

namespace Velo.Logging
{
    public static class LoggingExtensions
    {
        public static DependencyCollection AddLogging(this DependencyCollection collection)
        {
            collection
                .AddSingleton<IRendererCollection, RendererCollection>()
                .AddScoped(ctx => ctx.GetServices<ILogWriter>().Length > 0
                    ? (ILogProvider) ctx.Activate<LogProvider>()
                    : new NullProvider())
                .AddScoped(typeof(ILogger<>), typeof(Logger<>));

            return collection;
        }

        public static DependencyCollection AddDefaultLogEnrichers(this DependencyCollection collection)
        {
            collection
                .AddInstance<ILogEnricher>(new LevelEnricher())
                .AddInstance<ILogEnricher>(new SenderEnricher())
                .AddInstance<ILogEnricher>(new TimeStampEnricher());

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

        public static DependencyCollection AddLogEnricher(this DependencyCollection collection, ILogEnricher logEnricher)
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