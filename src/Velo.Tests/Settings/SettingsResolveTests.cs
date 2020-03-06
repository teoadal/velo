using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.Settings;
using Velo.TestsModels.Settings;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings
{
    public sealed class SettingsResolveTests : TestClass
    {
        private const string LogLevelNode = "Logging.LogLevel";

        private readonly ISettings _settings;
        private readonly DependencyProvider _provider;

        public SettingsResolveTests(ITestOutputHelper output) : base(output)
        {
            _provider = new DependencyCollection()
                .AddSettings()
                .AddJsonSettings("appsettings.json", true)
                .BuildProvider();

            _settings = _provider.GetRequiredService<ISettings>();
        }

        [Fact]
        public void Resolve()
        {
            var settings = _provider.GetRequiredService<LogLevelSettings>();
            Assert.NotNull(settings);
            Assert.Same(settings, _settings.Get<LogLevelSettings>(LogLevelNode));
        }

        [Fact]
        public async Task Resolve_MultiThreading()
        {
            await RunTasks(10, () =>
            {
                var settings = _provider.GetRequiredService<LogLevelSettings>();
                Assert.NotNull(settings);
                Assert.Same(settings, _settings.Get<LogLevelSettings>(LogLevelNode));
            });
        }
    }
}