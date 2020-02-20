using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.Extensions.DependencyInjection.Mapping;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Mapping
{
    public class ServiceCollectionShould : TestClass
    {
        private readonly IServiceCollection _serviceCollection;
        
        public ServiceCollectionShould(ITestOutputHelper output) : base(output)
        {
            _serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void RegisterMapper()
        {
            var provider = _serviceCollection
                .AddMapper()
                .BuildServiceProvider();

            var mapper = provider.GetRequiredService<IMapper<Foo>>();
            mapper.Should().NotBeNull();
        }
    }
}