using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Velo.Extensions.DependencyInjection;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.Extensions.DependencyInjection
{
    public class ServiceProviderExtensionsShould : TestClass
    {
        [Fact]
        public void GetArray()
        {
            var mocks = Many(Mock.Of<IBooRepository>);

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