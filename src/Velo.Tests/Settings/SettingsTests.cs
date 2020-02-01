using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Settings;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Settings
{
    public class SettingsTests : TestClass
    {
        private const string LogLevelNode = "Logging.LogLevel";

        private readonly IConfiguration _configuration;

        public SettingsTests(ITestOutputHelper output) : base(output)
        {
            var provider = new DependencyCollection()
                .AddJsonConfiguration("appsettings.json", true)
                .BuildProvider();

            _configuration = provider.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public void Contains()
        {
            Assert.True(_configuration.Contains(LogLevelNode));
            Assert.True(_configuration.Contains(LogLevelNode.Split('.')[0]));
            Assert.False(_configuration.Contains("abc"));
            Assert.False(_configuration.Contains("Logging.Abc"));
        }

        [Fact]
        public void GetObject()
        {
            var logSettings = _configuration.Get<LogLevelSettings>(LogLevelNode);

            Assert.Equal("Information", logSettings.Default);
            Assert.Equal("Information", logSettings.Microsoft);
            Assert.Equal("Information", logSettings.System);
        }

        [Fact]
        public async Task GetObject_MultiThreading()
        {
            await RunTasks(10, () =>
            {
                var logSettings = _configuration.Get<LogLevelSettings>(LogLevelNode);
                Assert.Equal("Information", logSettings.Default);
                Assert.Equal("Information", logSettings.Microsoft);
                Assert.Equal("Information", logSettings.System);
            });
        }

        [Fact]
        public void GetObject_Try()
        {
            Assert.True(_configuration.TryGet<LogLevelSettings>(LogLevelNode, out var logSettings));

            Assert.Equal("Information", logSettings.Default);
            Assert.Equal("Information", logSettings.Microsoft);
            Assert.Equal("Information", logSettings.System);
            
            Assert.False(_configuration.TryGet<LogLevelSettings>("Logging.Abc", out _));
        }
        
        [Fact]
        public void Override_CommandLineArgs()
        {
            var provider = new DependencyCollection()
                .AddJsonConfiguration("appsettings.json", true)
                .AddCommandLineConfiguration(new[]
                {
                    $"{LogLevelNode}.Default=\"One\"",
                    $"{LogLevelNode}.System=\"Two\"",
                    $"{LogLevelNode}.Microsoft=\"Three\""
                })
                .BuildProvider();

            var configuration = provider.GetRequiredService<IConfiguration>();

            Assert.Equal("One", configuration.Get<string>($"{LogLevelNode}.Default"));
            Assert.Equal("Two", configuration.Get<string>($"{LogLevelNode}.System"));
            Assert.Equal("Three", configuration.Get<string>($"{LogLevelNode}.Microsoft"));
        }

        [Fact]
        public void Override_JsonFileSettings()
        {
            var provider = new DependencyCollection()
                .AddJsonConfiguration("appsettings.json", true)
                .AddJsonConfiguration("appsettings.develop.json", true)
                .BuildProvider();

            var configuration = provider.GetRequiredService<IConfiguration>();

            Assert.Equal("Debug", configuration.Get($"{LogLevelNode}.Default"));
            Assert.Equal("Debug", configuration.Get<string>($"{LogLevelNode}.Default"));

            Assert.Equal("Debug", configuration.Get<string>($"{LogLevelNode}.System"));
            Assert.Equal("Information", configuration.Get<string>($"{LogLevelNode}.Microsoft"));

            Assert.Equal(1, configuration.Get<int>("Logging.IntValue"));
        }

        [Fact]
        public void Read_JsonFileSettings()
        {
            Assert.Equal("Information", _configuration.Get($"{LogLevelNode}.Default"));
            Assert.Equal("Information", _configuration.Get<string>($"{LogLevelNode}.Default"));

            Assert.Equal("Information", _configuration.Get<string>($"{LogLevelNode}.System"));
            Assert.Equal("Information", _configuration.Get<string>($"{LogLevelNode}.Microsoft"));

            Assert.Equal(1, _configuration.Get<int>("Logging.IntValue"));
        }

        [Theory, AutoData]
        public void Register_ByFunction(string fileNamePart)
        {
            var provider = new DependencyCollection()
                .AddInstance(new Boo {String = fileNamePart})
                .AddJsonConfiguration(ctx =>
                {
                    var boo = ctx.GetRequiredService<Boo>();
                    return $"appsettings.{boo.String}.json";
                })
                .BuildProvider();

            var configuration = provider.GetRequiredService<IConfiguration>();
            Assert.NotNull(configuration);
        }

        [Fact]
        public void Throw_CastPrimitiveToObject()
        {
            Assert.Throws<InvalidCastException>(() => _configuration.Get<int>(LogLevelNode));
        }

        [Fact]
        public void Throw_CastObjectToPrimitive()
        {
            Assert.Throws<InvalidCastException>(() =>
                _configuration.Get<LogLevelSettings>($"{LogLevelNode}.Microsoft"));
        }

        [Fact]
        public void Throw_PathNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                _configuration.Get<LogLevelSettings>($"abc.def"));
        }

        [Fact]
        public void Throw_RequiredFileNotFound()
        {
            var provider = new DependencyCollection()
                .AddJsonConfiguration("abc.json", true)
                .BuildProvider();

            Assert.Throws<FileNotFoundException>(() => provider.GetRequiredService<IConfiguration>());
        }
    }
}