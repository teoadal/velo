using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Configuration;
using Velo.Serialization;
using Velo.Settings.Provider;
using Velo.Settings.Sources;
using Velo.TestsModels.Settings;

namespace Velo.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SettingsBenchmark
    {
        private const string LogLevelNode = "Logging.LogLevel";

        private IConfigurationRoot _coreConfiguration;
        private IConfigurationSection _coreConfigurationSection;
        private SettingsProvider _veloConfiguration;
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

        [BenchmarkCategory("Cold")]
        [Benchmark(Baseline = true)]
        public int Core_Cold()
        {
            var configuration = BuildCoreConfiguration();
            var settings = configuration
                .GetSection("Logging")
                .GetSection("LogLevel")
                .Get<LogLevelSettings>();

            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Cold")]
        [Benchmark]
        public int Velo_Cold()
        {
            var configuration = BuildVeloConfiguration();
            var settings = configuration.Get<LogLevelSettings>(LogLevelNode);

            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Get")]
        [Benchmark(Baseline = true)]
        public int Core_Get()
        {
            var settings = _coreConfiguration
                .GetSection("Logging")
                .GetSection("LogLevel").Get<LogLevelSettings>();

            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Get")]
        [Benchmark]
        public int Core_GetSection()
        {
            var settings = _coreConfigurationSection.Get<LogLevelSettings>();
            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Get")]
        [Benchmark]
        public int Velo_Get()
        {
            var settings = _veloConfiguration.Get<LogLevelSettings>(LogLevelNode);
            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Reload")]
        [Benchmark(Baseline = true)]
        public int Core_Reload()
        {
            _coreConfiguration.Reload();

            var settings = _coreConfiguration
                .GetSection("Logging")
                .GetSection("LogLevel").Get<LogLevelSettings>();

            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Reload")]
        [Benchmark]
        public int Core_ReloadSection()
        {
            _coreConfiguration.Reload();

            var settings = _coreConfigurationSection.Get<LogLevelSettings>();
            return settings.Default.Length + settings.Microsoft.Length + settings.System.Length;
        }

        [BenchmarkCategory("Reload")]
        [Benchmark]
        public int Velo_Reload()
        {
            _veloConfiguration.Reload();

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

        private SettingsProvider BuildVeloConfiguration()
        {
            var converters = new ConvertersCollection();
            return new SettingsProvider(new ISettingsSource[]
            {
                new JsonFileSource("appsettings.json", true),
                new JsonFileSource("appsettings.develop.json", true),
                new CommandLineSource(_commandLineArgs),
            }, converters);
        }
    }
}