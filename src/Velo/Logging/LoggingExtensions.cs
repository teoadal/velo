using Velo.DependencyInjection;
using Velo.Logging.Enrichers;
using Velo.Logging.Writers;
using Velo.Utils;

namespace Velo.Logging
{
    public static class LoggingExtensions
    {
        public static DependencyCollection AddLogging(this DependencyCollection collection)
        {
            collection
                .AddSingleton<LogProvider>()
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            return collection;
        }

        public static DependencyCollection AddDefaultLogEnrichers(this DependencyCollection collection)
        {
            collection
                .AddSingleton<ILogEnricher, LogLevelEnricher>()
                .AddSingleton<ILogEnricher, SenderEnricher>()
                .AddSingleton<ILogEnricher, TimestampEnricher>();
            
            return collection;
        }
        
        public static DependencyCollection AddLogWriter<TWriter>(this DependencyCollection collection)
            where TWriter : ILogWriter
        {
            var implementation = Typeof<TWriter>.Raw;
            var contracts = new [] { Typeof<ILogWriter>.Raw, implementation };

            collection.AddDependency(contracts, implementation, DependencyLifetime.Singleton);
            
            return collection;
        }
        
        public static DependencyCollection AddLogWriter(this DependencyCollection collection, ILogWriter logWriter)
        {
            collection.AddInstance(logWriter);
            return collection;
        }
    }
}