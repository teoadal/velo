using FluentAssertions;
using Velo.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection
{
    public class DependencyExtensionsShould : TestClass
    {
        public DependencyExtensionsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData(new [] { DependencyLifetime.Singleton }, DependencyLifetime.Singleton)]
        [InlineData(new [] { DependencyLifetime.Singleton, DependencyLifetime.Singleton }, DependencyLifetime.Singleton)]
        [InlineData(new [] { DependencyLifetime.Singleton, DependencyLifetime.Scoped }, DependencyLifetime.Scoped)]
        [InlineData(new [] { DependencyLifetime.Singleton, DependencyLifetime.Scoped, DependencyLifetime.Transient }, DependencyLifetime.Transient)]
        [InlineData(new [] { DependencyLifetime.Singleton, DependencyLifetime.Transient }, DependencyLifetime.Transient)]
        [InlineData(new [] { DependencyLifetime.Scoped, DependencyLifetime.Transient }, DependencyLifetime.Transient)]
        [InlineData(new [] { DependencyLifetime.Scoped, DependencyLifetime.Scoped }, DependencyLifetime.Scoped)]
        [InlineData(new [] { DependencyLifetime.Scoped }, DependencyLifetime.Scoped)]
        [InlineData(new [] { DependencyLifetime.Transient }, DependencyLifetime.Transient)]
        public void ValidDefine(DependencyLifetime[] lifetimes, DependencyLifetime expected)
        {
            lifetimes.DefineLifetime().Should().Be(expected);
        }
    }
}