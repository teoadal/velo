using NLog.Targets;
using Serilog;
using Serilog.Core;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Writers;
using ILogger = Serilog.ILogger;
using LogLevel = NLog.LogLevel;

namespace Velo.Benchmark.Logging
{
    public static class LoggerBuilder
    {
        public static NLog.Logger BuildNLog(params Target[] targets)
        {
            var config = new NLog.Config.LoggingConfiguration();

            foreach (var target in targets)
            {
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, target);
                config.AddTarget(target);    
            }
            
            NLog.LogManager.Configuration = config;

            return config.LogFactory.GetLogger("test", typeof(LoggerBenchmark));
        }

        public static ILogger BuildSerilog(ILogEventSink sink)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sink(sink)
                .CreateLogger()
                .ForContext<LoggerBenchmark>();
        }

        public static ILogger<LoggerBenchmark> BuildVelo(ILogWriter logWriter)
        {
            return new DependencyCollection()
                .AddLogging()
                .AddLogWriter(logWriter)
                .AddLogEnricher<TimeStampEnricher>()
                .AddLogEnricher<LevelEnricher>()
                .BuildProvider()
                .GetRequiredService<ILogger<LoggerBenchmark>>();
        }
    }
}