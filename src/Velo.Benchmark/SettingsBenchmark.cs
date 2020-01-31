using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Configuration;
using Velo.Settings;
using Velo.Settings.Sources;
using Velo.TestsModels.Settings;

namespace Velo.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SettingsBenchmark
    {
        private const string LogLevelNode = "Logging.LogLevel";

        private IConfigurationRoot _coreConfiguration;
        private IConfigurationSection _coreConfigurationSection;
        private Configuration _veloConfiguration;
        private string[] _commandLineArgs;

        [GlobalSetup]
        public void Init()
        {
            _commandLineArgs = new[]
            {
                $"{LogLevelNode}.Default=\"One\"",
                $"{LogLevelNode}.System=\"Two\"",
                $"{LogLevelNode}.Microsoft=\"Three\""
            };

            _coreConfiguration = BuildCoreConfiguration();
            _coreConfigurationSection = _coreConfiguration.GetSection("Logging").GetSection("LogLevel");
            _veloConfiguration = BuildVeloConfiguration();
        }

        [BenchmarkCategory("Build")]
        [Benchmark(Baseline = true)]
        public int Core_Build()
        {
            var configuration = BuildCoreConfiguration();
            return configuration.Providers.Count();
        }

        [BenchmarkCategory("Build")]
        [Benchmark]
        public int Velo_Build()
        {
            var configuration = BuildVeloConfiguration();
            return configuration.Sources.Length;
        }

        [BenchmarkCategory("Get")]
        [Benchmark(Baseline = true, OperationsPerInvoke = 10)]
        public int Core_Get()
        {
            var settings = _coreConfiguration
                .GetSection("Logging")
                .GetSection("LogLevel").Get<LogLevelSettings>();

            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Get")]
        [Benchmark(OperationsPerInvoke = 10)]
        public int Core_Get_Section()
        {
            var settings = _coreConfigurationSection.Get<LogLevelSettings>();
            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Get")]
        [Benchmark(OperationsPerInvoke = 10)]
        public int Velo_Get()
        {
            var settings = _veloConfiguration.Get<LogLevelSettings>(LogLevelNode);
            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        private IConfigurationRoot BuildCoreConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.develop.json")
                .AddCommandLine(_commandLineArgs)
                .Build();
        }

        private Configuration BuildVeloConfiguration()
        {
            return new Configuration(new Settings.Sources.IConfigurationSource[]
            {
                new JsonFileSource("appsettings.json", true),
                new JsonFileSource("appsettings.develop.json", true),
                new CommandLineSource(_commandLineArgs),
            });
        }
    }
}