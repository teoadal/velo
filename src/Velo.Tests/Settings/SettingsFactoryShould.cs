using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Settings;
using Velo.TestsModels.Settings;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings
{
    public class SettingsFactoryShould : TestClass
    {
        private readonly IDependencyEngine _engine;
        private readonly SettingsFactory _factory;
        private readonly Type _settingsType;

        public SettingsFactoryShould(ITestOutputHelper output) : base(output)
        {
            _engine = Mock.Of<IDependencyEngine>();
            _factory = new SettingsFactory();
            _settingsType = typeof(LogLevelSettings);
        }

        [Fact]
        public void Applicable()
        {
            var result = _factory.Applicable(_settingsType);
            result.Should().BeTrue();
        }

        [Fact]
        public void BuildDependency()
        {
            var dependency = _factory.BuildDependency(_settingsType, _engine);
            dependency.Should().NotBeNull();
        }

        [Fact]
        public void BuildValidDependency()
        {
            var dependency = _factory.BuildDependency(_settingsType, _engine);

            dependency.Contracts.Should().Contain(_settingsType);
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
        }

        [Fact]
        public void NotApplicable()
        {
            var result = _factory.Applicable(typeof(SettingsWithoutAttribute));
            result.Should().BeFalse();
        }

        [Fact]
        public void BuildResolvable()
        {
            var settings = new DependencyCollection()
                .AddSettings()
                .AddJsonSettings("Settings/appsettings.json")
                .BuildProvider()
                .GetRequiredService<LogLevelSettings>();

            settings.Default.Should().NotBeNullOrWhiteSpace();
            settings.Microsoft.Should().NotBeNullOrWhiteSpace();
            settings.System.Should().NotBeNullOrWhiteSpace();
        }
        
        private sealed class SettingsWithoutAttribute
        {
        }
    }
}