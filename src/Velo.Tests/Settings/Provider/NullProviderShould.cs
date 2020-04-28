using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Settings.Provider;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings.Provider
{
    public class NullProviderShould : TestClass
    {
        private readonly NullSettingsProvider _provider;

        public NullProviderShould(ITestOutputHelper output) : base(output)
        {
            _provider = new NullSettingsProvider();
        }

        [Theory]
        [AutoData]
        public void NotContain(string path)
        {
            _provider.Contains(path).Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public void NotReturnValue(string path)
        {
            _provider.TryGet<Boo>(path, out _).Should().BeFalse();
        }

        [Fact]
        public void ReloadNotAffect()
        {
            _provider
                .Invoking(provider => provider.Reload())
                .Should().NotThrow();
        }

        [Theory]
        [AutoData]
        public void ThrowGet(string path)
        {
            _provider
                .Invoking(provider => provider.Get(path))
                .Should().Throw<KeyNotFoundException>();
        }

        [Theory]
        [AutoData]
        public void ThrowGetObject(string path)
        {
            _provider
                .Invoking(provider => provider.Get<Boo>(path))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}