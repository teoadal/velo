using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection.CQRS
{
    public class EmitterInstallerShould : TestClass
    {
        private readonly IServiceCollection _services;

        public EmitterInstallerShould(ITestOutputHelper output) : base(output)
        {
            _services = new ServiceCollection()
                .AddEmitter();
        }

        [Fact]
        public void RegisterEmitter()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IEmitter) &&
                descriptor.ImplementationFactory != null);
        }

        [Fact]
        public void RegisterScopedEmitter()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IEmitter) &&
                descriptor.Lifetime == ServiceLifetime.Scoped);
        }

        [Fact]
        public void ResolveEmitter()
        {
            _services
                .BuildServiceProvider()
                .GetService<IEmitter>()
                .Should().NotBeNull();
        }
    }
}