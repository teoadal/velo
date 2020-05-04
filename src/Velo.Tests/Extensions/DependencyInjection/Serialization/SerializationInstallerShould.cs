using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.Serialization;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection.Serialization
{
    public class SerializationInstallerShould : TestClass
    {
        private readonly IServiceCollection _services;

        public SerializationInstallerShould(ITestOutputHelper output) : base(output)
        {
            _services = new ServiceCollection()
                .AddJsonConverter();
        }

        [Fact]
        public void InstallConvertersCollection()
        {
            _services.Should().Contain(descriptor => 
                    descriptor.ServiceType == typeof(IConvertersCollection) &&
                    descriptor.Lifetime == ServiceLifetime.Singleton);
        }
        
        [Fact]
        public void InstallConverter()
        {
            _services.Should().Contain(descriptor => 
                    descriptor.ServiceType == typeof(JConverter) &&
                    descriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void InstallSingletonConverter()
        {
            _services.Should().Contain(descriptor => 
                descriptor.ServiceType == typeof(JConverter) &&
                descriptor.Lifetime == ServiceLifetime.Singleton);
        }
        
        [Fact]
        public void ResolveConverter()
        {
            _services
                .BuildServiceProvider()
                .GetService<JConverter>()
                .Should().NotBeNull();
        }
        
        [Fact]
        public void ResolveConvertersCollection()
        {
            _services
                .BuildServiceProvider()
                .GetService<IConvertersCollection>()
                .Should().NotBeNull();
        }
    }
}