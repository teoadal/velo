using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings.Provider;
using Velo.Settings.Sources;
using Xunit;

namespace Velo.Tests.Settings
{
    public class SettingsInstallerShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public SettingsInstallerShould()
        {
            _dependencies = new DependencyCollection()
                .AddSettings();
        }

        [Fact]
        public void AddCommandLineSource()
        {
            _dependencies.AddCommandLineSettings(It.IsNotNull<string[]>());
            _dependencies.Contains<ISettingsSource>().Should().BeTrue();
        }

        [Fact]
        public void AddEnvironmentSource()
        {
            _dependencies.AddEnvironmentSettings();
            _dependencies.Contains<ISettingsSource>().Should().BeTrue();
        }

        [Fact]
        public void AddJsonSource()
        {
            _dependencies.AddJsonSettings();
            _dependencies.Contains<ISettingsSource>().Should().BeTrue();
        }

        [Fact]
        public void AddJsonSourceBuilder()
        {
            _dependencies.AddJsonSettings(ctx => ctx.ToString());
            _dependencies.Contains<ISettingsSource>().Should().BeTrue();
        }
        
        [Fact]
        public void InstallProvider()
        {
            _dependencies
                .BuildProvider()
                .GetRequired<ISettingsProvider>()
                .Should().NotBeNull();
        }

        [Fact]
        public void InstallJson()
        {
            _dependencies.Contains<JConverter>().Should().BeTrue();
        }
    }
}