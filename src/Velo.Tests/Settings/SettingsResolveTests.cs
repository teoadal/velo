using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.TestsModels.Settings;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Settings
{
    public sealed class SettingsResolveTests : TestClass
    {
        private const string LogLevelNode = "Logging.LogLevel";

        private readonly IConfiguration _configuration;
        private readonly DependencyProvider _provider;

        public SettingsResolveTests(ITestOutputHelper output) : base(output)
        {
            _provider = new DependencyCollection()
                .AddJsonConfiguration("appsettings.json", true)
                .BuildProvider();

            _configuration = _provider.GetRequiredService<IConfiguration>();
        }

        [Fact]
        public void Resolve()
        {
            var settings = _provider.GetRequiredService<LogLevelSettings>();
            Assert.NotNull(settings);
            Assert.Same(settings, _configuration.Get<LogLevelSettings>(LogLevelNode));
        }

        [Fact]
        public async Task Resolve_MultiThreading()
        {
            await RunTasks(10, () =>
            {
                var settings = _provider.GetRequiredService<LogLevelSettings>();
                Assert.NotNull(settings);
                Assert.Same(settings, _configuration.Get<LogLevelSettings>(LogLevelNode));
            });
        }
    }
}