using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Settings;
using Velo.Settings.Provider;
using Velo.Settings.Sources;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings.Provider
{
    public class SettingsProviderFactoryShould : TestClass
    {
        private readonly Type _contract;
        private readonly Mock<IDependencyEngine> _engine;
        private readonly SettingsProviderFactory _factory;

        public SettingsProviderFactoryShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(ISettings);
            _engine = new Mock<IDependencyEngine>();
            _factory = new SettingsProviderFactory();
        }

        [Fact]
        public void Applicable()
        {
            _factory
                .Applicable(_contract)
                .Should().BeTrue();
        }

        [Fact]
        public void BuildSettingsDependency()
        {
            var dependency = _factory.BuildDependency(_contract, _engine.Object);
            dependency.Contracts.Should().Contain(_contract);
        }

        [Fact]
        public void BuildNullProvider()
        {
            var dependency = _factory.BuildDependency(_contract, _engine.Object);
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
            dependency.Resolver.Implementation.Should().Be(typeof(NullProvider));
        }

        [Fact]
        public void BuildSettingsProvider()
        {
            _engine
                .Setup(engine => engine.Contains(typeof(ISettingsSource)))
                .Returns(true);

            var dependency = _factory.BuildDependency(_contract, _engine.Object);
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
            dependency.Resolver.Implementation.Should().Be(typeof(SettingsProvider));
        }

        [Fact]
        public void NotApplicable()
        {
            _factory
                .Applicable(typeof(Boo))
                .Should().BeFalse();
        }
    }
}