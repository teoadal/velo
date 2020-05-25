using System;
using Velo.DependencyInjection.Dependencies;
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
        private static readonly Type[] LogWriterContracts = {Typeof<ILogWriter>.Raw};

        public static DependencyCollection AddLogging(this DependencyCollection dependencies)
        {
            dependencies
                .AddSingleton<IRenderersCollection, RenderersCollection>()
                .AddFactory<ILogProvider, LogProvider>(factory => factory
                    .Lifetime(DependencyLifetime.Scoped)
                    .CreateIf<NullLogProvider>(engine => !engine.Contains(typeof(ILogWriter))))
                .AddScoped(typeof(ILogger<>), typeof(Logger<>))
                .EnsureJsonEnabled();

            return dependencies;
        }

        public static DependencyCollection AddDefaultConsoleLogWriter(this DependencyCollection dependencies,
            LogLevel level = LogLevel.Debug)
        {
            var dependency = new InstanceDependency(LogWriterContracts, new DefaultConsoleWriter(level));
            dependencies.Add(dependency);

            return dependencies;
        }

        public static DependencyCollection AddDefaultFileLogWriter(this DependencyCollection dependencies,
            string? filePath = null, LogLevel level = LogLevel.Debug)
        {
            filePath ??= $"{AppDomain.CurrentDomain.FriendlyName}.log";

            var dependency = new InstanceDependency(LogWriterContracts, new DefaultFileWriter(filePath, level));
            dependencies.Add(dependency);

            return dependencies;
        }

        public static DependencyCollection AddDefaultLogEnrichers(this DependencyCollection dependencies)
        {
            AddLogEnricher<LogLevelEnricher>(dependencies);
            AddLogEnricher<SenderEnricher>(dependencies);
            AddLogEnricher<TimeStampEnricher>(dependencies);

            return dependencies;
        }

        public static DependencyCollection AddLogEnricher<TEnricher>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TEnricher : ILogEnricher
        {
            var implementation = Typeof<TEnricher>.Raw;
            var contracts = new[] {Typeof<ILogEnricher>.Raw, implementation};

            dependencies.AddDependency(contracts, implementation, lifetime);

            return dependencies;
        }

        public static DependencyCollection AddLogEnricher(this DependencyCollection dependencies,
            ILogEnricher logEnricher)
        {
            dependencies.AddInstance(logEnricher);
            return dependencies;
        }

        public static DependencyCollection AddLogWriter<TWriter>(this DependencyCollection dependencies,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where TWriter : ILogWriter
        {
            var implementation = Typeof<TWriter>.Raw;
            var contracts = new[] {Typeof<ILogWriter>.Raw, implementation};

            dependencies.AddDependency(contracts, implementation, lifetime);

            return dependencies;
        }

        public static DependencyCollection AddLogWriter(this DependencyCollection dependencies, ILogWriter logWriter)
        {
            dependencies.AddInstance(logWriter);
            return dependencies;
        }
    }
}