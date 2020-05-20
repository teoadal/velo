using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.Mapping;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;

namespace Velo.Tests.Extensions.DependencyInjection.Mapping
{
    public class MappingInstallerShould : TestClass
    {
        private readonly IServiceCollection _services;
        
        public MappingInstallerShould()
        {
            _services = new ServiceCollection()
                .AddMapper();
        }

        [Fact]
        public void InstallMapper()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IMapper<>) &&
                descriptor.ImplementationType == typeof(CompiledMapper<>));
        }
        
        [Fact]
        public void InstallSingletonMapper()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IMapper<>) && 
                descriptor.Lifetime == ServiceLifetime.Singleton);
        }
        
        [Fact]
        public void ResolveMapper()
        {
            _services
                .BuildServiceProvider()
                .GetService<IMapper<Boo>>()
                .Should().NotBeNull();
        }
        
        [Fact]
        public void ResolveManyMappers()
        {
            var provider = _services.BuildServiceProvider();
            
            provider
                .GetService<IMapper<Boo>>()
                .Should().BeOfType<CompiledMapper<Boo>>();
            
            provider
                .GetService<IMapper<Foo>>()
                .Should().BeOfType<CompiledMapper<Foo>>();
        }
    }
}