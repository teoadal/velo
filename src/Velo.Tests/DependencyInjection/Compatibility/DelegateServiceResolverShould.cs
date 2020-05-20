using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection.Compatibility;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Compatibility
{
    public class DelegateServiceResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Mock<Func<Type, object>> _resolver;
        private readonly IServiceProvider _services;

        public DelegateServiceResolverShould()
        {
            _contract = typeof(IBooRepository);
            _resolver = new Mock<Func<Type, object>>();
            _services = new DelegateServiceResolver(_resolver.Object);
        }

        [Fact]
        public void CallResolver()
        {
            _services
                .Invoking(provider => provider.GetService(_contract))
                .Should().NotThrow();

            _resolver.Verify(resolver => resolver.Invoke(_contract));
        }

        [Fact]
        public void GetService()
        {
            var instance = Mock.Of<IBooRepository>();

            _resolver
                .Setup(resolver => resolver.Invoke(_contract))
                .Returns(instance);

            var actual = _services.GetService(_contract);
            actual.Should().Be(instance);
        }
    }
}