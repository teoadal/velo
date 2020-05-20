using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public class SingletonDependencyShould : DITestClass
    {
        private readonly Type _contract;
        private readonly SingletonDependency _dependency;
        private readonly IServiceProvider _services;

        private Mock<IBooRepository> _instance;
        
        public SingletonDependencyShould()
        {
            _contract = typeof(IBooRepository);

            var resolver = MockResolver(_contract, (type, scope) =>
            {
                _instance = new Mock<IDisposable>().As<IBooRepository>();
                return _instance.Object;
            });

            _dependency = new SingletonDependency(new[] {_contract}, resolver.Object);

            _services = Mock.Of<IServiceProvider>();
        }

        [Fact]
        public void GetInstance()
        {
            var instance = _dependency.GetInstance(_contract, _services);
            instance.Should().Be(_instance.Object);
        }
        
        [Fact]
        public void GetSingleInstance()
        {
            var first = _dependency.GetInstance(_contract, _services);
            var second = _dependency.GetInstance(_contract, _services);

            first.Should().Be(second);
        }
        
        [Fact]
        public void DisposeInstance()
        {
            _dependency.GetInstance(_contract, _services);
            
            _dependency.Dispose();
            _instance.As<IDisposable>().Verify(i => i.Dispose());
        }
    }
}