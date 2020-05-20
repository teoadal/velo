using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public class TransientDependencyShould : DITestClass
    {
        private readonly Type _contract;
        private readonly TransientDependency _dependency;
        private readonly IServiceProvider _services;

        private Mock<IBooRepository> _instance;

        public TransientDependencyShould()
        {
            _contract = typeof(IBooRepository);

            var resolver = MockResolver(_contract, (type, scope) =>
            {
                _instance = new Mock<IDisposable>().As<IBooRepository>();
                return _instance.Object;
            });

            _dependency = new TransientDependency(new[] {_contract}, resolver.Object);

            _services = Mock.Of<IServiceProvider>();
        }

        [Fact]
        public void GetInstance()
        {
            var instance = _dependency.GetInstance(_contract, _services);
            instance.Should().Be(_instance.Object);
        }

        [Fact]
        public void GetManyInstances()
        {
            var first = _dependency.GetInstance(_contract, _services);
            var second = _dependency.GetInstance(_contract, _services);

            first.Should().NotBe(second);
        }

        [Fact]
        public void NotDisposeInstance()
        {
            _dependency.GetInstance(_contract, _services);

            _dependency.Dispose();

            _instance.As<IDisposable>().Verify(i => i.Dispose(), Times.Never);
        }
    }
}