using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Settings;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings
{
    public class SettingsTests : TestClass
    {
        private const string LogLevelNode = "Logging.LogLevel";

        private readonly ISettings _settings;

        public SettingsTests(ITestOutputHelper output) : base(output)
        {
            var provider = new DependencyCollection()
                .AddSettings()
                .AddJsonSettings("appsettings.json", true)
                .BuildProvider();

            _settings = provider.GetRequiredService<ISettings>();
        }

        [Fact]
        public void Contains()
        {
            Assert.True(_settings.Contains(LogLevelNode));
            Assert.True(_settings.Contains(LogLevelNode.Split('.')[0]));
            Assert.False(_settings.Contains("abc"));
            Assert.False(_settings.Contains("Logging.Abc"));
        }

        [Fact]
        public void GetObject()
        {
            var logSettings = _settings.Get<LogLevelSettings>(LogLevelNode);

            Assert.Equal("Information", logSettings.Default);
            Assert.Equal("Information", logSettings.Microsoft);
            Assert.Equal("Information", logSettings.System);
        }

        [Fact]
        public async Task GetObject_MultiThreading()
        {
            await RunTasks(10, () =>
            {
                var logSettings = _settings.Get<LogLevelSettings>(LogLevelNode);
                Assert.Equal("Information", logSettings.Default);
                Assert.Equal("Information", logSettings.Microsoft);
                Assert.Equal("Information", logSettings.System);
            });
        }

        [Fact]
        public void GetObject_Try()
        {
            Assert.True(_settings.TryGet<LogLevelSettings>(LogLevelNode, out var logSettings));

            Assert.Equal("Information", logSettings.Default);
            Assert.Equal("Information", logSettings.Microsoft);
            Assert.Equal("Information", logSettings.System);

            Assert.False(_settings.TryGet<LogLevelSettings>("Logging.Abc", out _));
        }

        [Fact]
        public void Override_CommandLineArgs()
        {
            var provider = new DependencyCollection()
                .AddSettings()
                .AddJsonSettings("appsettings.json", true)
                .AddCommandLineSettings(new[]
                {
                    $"{LogLevelNode}.Default=\"One\"",
                    $"{LogLevelNode}.System=\"Two\"",
                    $"{LogLevelNode}.Microsoft=\"Three\""
                })
                .BuildProvider();

            var configuration = provider.GetRequiredService<ISettings>();

            Assert.Equal("One", configuration.Get<string>($"{LogLevelNode}.Default"));
            Assert.Equal("Two", configuration.Get<string>($"{LogLevelNode}.System"));
            Assert.Equal("Three", configuration.Get<string>($"{LogLevelNode}.Microsoft"));
        }

        [Fact]
        public void Override_JsonFileSettings()
        {
            var provider = new DependencyCollection()
                .AddSettings()
                .AddJsonSettings("appsettings.json", true)
                .AddJsonSettings("appsettings.develop.json", true)
                .BuildProvider();

            var configuration = provider.GetRequiredService<ISettings>();

            Assert.Equal("Debug", configuration.Get($"{LogLevelNode}.Default"));
            Assert.Equal("Debug", configuration.Get<string>($"{LogLevelNode}.Default"));

            Assert.Equal("Debug", configuration.Get<string>($"{LogLevelNode}.System"));
            Assert.Equal("Information", configuration.Get<string>($"{LogLevelNode}.Microsoft"));

            Assert.Equal(1, configuration.Get<int>("Logging.IntValue"));
        }

        [Fact]
        public void Read_JsonFileSettings()
        {
            Assert.Equal("Information", _settings.Get($"{LogLevelNode}.Default"));
            Assert.Equal("Information", _settings.Get<string>($"{LogLevelNode}.Default"));

            Assert.Equal("Information", _settings.Get<string>($"{LogLevelNode}.System"));
            Assert.Equal("Information", _settings.Get<string>($"{LogLevelNode}.Microsoft"));

            Assert.Equal(1, _settings.Get<int>("Logging.IntValue"));
        }

        [Theory, AutoData]
        public void Register_ByFunction(string fileNamePart)
        {
            var provider = new DependencyCollection()
                .AddInstance(new Boo {String = fileNamePart})
                .AddSettings()
                .AddJsonSettings(ctx =>
                {
                    var boo = ctx.GetRequiredService<Boo>();
                    return $"appsettings.{boo.String}.json";
                })
                .BuildProvider();

            var configuration = provider.GetRequiredService<ISettings>();
            Assert.NotNull(configuration);
        }

        [Fact]
        public void Throw_CastPrimitiveToObject()
        {
            Assert.Throws<InvalidCastException>(() => _settings.Get<int>(LogLevelNode));
        }

        [Fact]
        public void Throw_CastObjectToPrimitive()
        {
            Assert.Throws<InvalidCastException>(() =>
                _settings.Get<LogLevelSettings>($"{LogLevelNode}.Microsoft"));
        }

        [Fact]
        public void Throw_PathNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                _settings.Get<LogLevelSettings>($"abc.def"));
        }

        [Fact]
        public void Throw_RequiredFileNotFound()
        {
            var provider = new DependencyCollection()
                .AddSettings()
                .AddJsonSettings("abc.json", true)
                .BuildProvider();

            Assert.Throws<FileNotFoundException>(() => provider.GetRequiredService<ISettings>());
        }
    }
}