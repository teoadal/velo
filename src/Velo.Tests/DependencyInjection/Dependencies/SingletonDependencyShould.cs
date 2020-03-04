using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public class SingletonDependencyShould : DITestClass
    {
        private readonly Type _contract;
        private readonly SingletonDependency _dependency;
        private readonly IDependencyScope _scope;

        private Mock<IBooRepository> _instance;
        
        public SingletonDependencyShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(IBooRepository);

            var resolver = MockResolver(_contract, (type, scope) =>
            {
                _instance = new Mock<IDisposable>().As<IBooRepository>();
                return _instance.Object;
            });

            _dependency = new SingletonDependency(new[] {_contract}, resolver.Object);

            _scope = MockScope().Object;
        }

        [Fact]
        public void GetInstance()
        {
            var instance = _dependency.GetInstance(_contract, _scope);
            instance.Should().Be(_instance.Object);
        }
        
        [Fact]
        public void GetSingleInstance()
        {
            var first = _dependency.GetInstance(_contract, _scope);
            var second = _dependency.GetInstance(_contract, _scope);

            first.Should().Be(second);
        }
        
        [Fact]
        public void DisposeInstance()
        {
            _dependency.GetInstance(_contract, _scope);
            
            _dependency.Dispose();
            _instance.As<IDisposable>().Verify(i => i.Dispose());
        }
    }
}