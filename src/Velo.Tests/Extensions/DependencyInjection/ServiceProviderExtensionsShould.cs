using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Velo.Extensions.DependencyInjection;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection
{
    public class ServiceProviderExtensionsShould : TestClass
    {
        public ServiceProviderExtensionsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void GetArray()
        {
            var mocks = BuildMany(5, () => new Mock<IBooRepository>().Object);

            var services = new ServiceCollection();

            foreach (var mock in mocks)
            {
                services.AddSingleton(mock);
            }

            services
                .BuildServiceProvider()
                .GetArray<IBooRepository>()
                .Should().Contain(mocks);
        }
    }
}