using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection
{
    public class DependencyEngineShould : DITestClass
    {
        private readonly Type _contract;

        private readonly DependencyEngine _engine;

        public DependencyEngineShould()
        {
            _contract = typeof(IBooRepository);
            _engine = new DependencyEngine(10);
        }

        [Fact]
        public void AddDependency()
        {
            var dependency = MockDependency(contract: _contract);
            _engine.AddDependency(dependency.Object);

            _engine.Contains(_contract).Should().BeTrue();
        }

        [Fact]
        public void AddFactory()
        {
            var factory = new Mock<IDependencyFactory>();
            factory
                .Setup(f => f.Applicable(_contract))
                .Returns(true);

            _engine.AddFactory(factory.Object);

            _engine.Contains(_contract).Should().BeTrue();
        }

        [Fact]
        public void IncludeArrayFactory()
        {
            var dependency = MockDependency(contract: _contract);
            _engine.AddDependency(dependency.Object);

            _engine
                .Contains(_contract.MakeArrayType())
                .Should().BeTrue();
        }

        [Fact]
        public void IncludeReferenceFactory()
        {
            var dependency = MockDependency(contract: _contract);
            _engine.AddDependency(dependency.Object);

            _engine
                .Contains(typeof(IReference<>).MakeGenericType(_contract))
                .Should().BeTrue();
        }

        [Fact]
        public void ContainsApplicableDependency()
        {
            var dependency = MockDependency(contract: _contract);
            _engine.AddDependency(dependency.Object);

            _engine.Contains(_contract).Should().BeTrue();
        }

        [Fact]
        public void ContainsApplicableFactory()
        {
            var factory = MockDependencyFactory(_contract);
            _engine.AddFactory(factory.Object);

            _engine.Contains(_contract).Should().BeTrue();
        }

        [Fact]
        public void DisposeAgainNotAffect()
        {
            for (var i = 0; i < 5; i++)
            {
                _engine
                    .Invoking(engine => engine.Dispose())
                    .Should().NotThrow();
            }
        }

        [Fact]
        public void DisposeDependencies()
        {
            var dependency = new Mock<IDisposable>().As<IDependency>();

            _engine.AddDependency(dependency.Object);
            _engine.Dispose();

            dependency.As<IDisposable>().Verify(d => d.Dispose());
        }

        [Fact]
        public void DisposeFactories()
        {
            var factory = new Mock<IDisposable>().As<IDependencyFactory>();

            _engine.AddFactory(factory.Object);

            _engine.Dispose();

            factory.As<IDisposable>().Verify(f => f.Dispose());
        }

        [Fact]
        public void GetApplicableDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _engine.AddDependency(dependency);

            _engine.GetApplicable(_contract).Should().Contain(dependency);
        }

        [Fact]
        public void GetApplicableFactory()
        {
            var dependency = MockDependency(contract: _contract).Object;
            var factory = MockDependencyFactory(_contract);
            factory
                .Setup(f => f.BuildDependency(_contract, _engine))
                .Returns(dependency);

            _engine.AddFactory(factory.Object);

            _engine.GetApplicable(_contract).Should().Contain(dependency);
        }

        [Fact]
        public void GetApplicableDependencyAndFactory()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _engine.AddDependency(dependency);

            var factoryDependency = MockDependency(contract: _contract).Object;
            var factory = MockDependencyFactory(_contract);
            factory
                .Setup(f => f.BuildDependency(_contract, _engine))
                .Returns(factoryDependency);

            _engine.AddFactory(factory.Object);

            _engine.GetApplicable(_contract)
                .Should().Contain(factoryDependency)
                .And.Contain(dependency);
        }

        [Fact]
        public void GetDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _engine.AddDependency(dependency);

            _engine.GetDependency(_contract).Should().Be(dependency);
        }

        [Fact]
        public void GetDependencyFromFactory()
        {
            var dependency = MockDependency(contract: _contract).Object;
            var factory = MockDependencyFactory(_contract);
            factory
                .Setup(f => f.BuildDependency(_contract, _engine))
                .Returns(dependency);

            _engine.AddFactory(factory.Object);

            _engine.GetDependency(_contract).Should().Be(dependency);
        }

        [Fact]
        public void GetDependencyCanReturnNull()
        {
            _engine.AddDependency(MockDependency(contract: _contract).Object);
            _engine.GetDependency(typeof(Boo)).Should().BeNull();
        }

        [Fact]
        public void GetDependencyAgainFromCache()
        {
            _engine.AddDependency(MockDependency(contract: _contract).Object);

            _engine.GetDependency(_contract).Should().Be(_engine.GetDependency(_contract));
        }

        [Fact]
        public void GetRequiredDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _engine.AddDependency(dependency);

            _engine.GetRequiredDependency(_contract).Should().Be(dependency);
        }

        [Fact]
        public void RemoveDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _engine.AddDependency(Mock.Of<IDependency>()); // other dependency
            _engine.AddDependency(dependency);

            var factoryDependency = MockDependency(contract: _contract).Object;
            var factory = MockDependencyFactory(_contract);
            factory
                .Setup(f => f.BuildDependency(_contract, _engine))
                .Returns(factoryDependency);

            _engine.AddFactory(factory.Object);

            _engine // cache dependency
                .Invoking(engine => engine.GetRequiredDependency(_contract))
                .Should().NotThrow();

            _engine.Remove(_contract).Should().BeTrue();
        }

        [Fact]
        public void RemoveDependencyFactory()
        {
            var factoryDependency = MockDependency(contract: _contract).Object;
            var factory = MockDependencyFactory(_contract);
            factory
                .Setup(f => f.BuildDependency(_contract, _engine))
                .Returns(factoryDependency);

            _engine.AddFactory(Mock.Of<IDependencyFactory>()); // other factory
            _engine.AddFactory(factory.Object);

            _engine.Remove(_contract).Should().BeTrue();
        }

        [Fact]
        public void NotContains()
        {
            _engine.Contains(typeof(Boo)).Should().BeFalse();
        }

        [Fact]
        public void NotThrowIfApplicableNotFound()
        {
            _engine
                .Invoking(engine => engine.GetApplicable(typeof(Boo)))
                .Should().NotThrow()
                .Which.Should().BeEmpty();
        }

        [Fact]
        public void NotThrowIfRemovedContractNotExists()
        {
            _engine
                .Invoking(engine => engine.Remove(typeof(Boo)))
                .Should().NotThrow()
                .Which.Should().BeFalse();
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _engine.Dispose();

            _engine
                .Invoking(engine => engine.Contains(It.IsNotNull<Type>()))
                .Should().Throw<ObjectDisposedException>();

            _engine
                .Invoking(engine => engine.GetApplicable(It.IsNotNull<Type>()))
                .Should().Throw<ObjectDisposedException>();

            _engine
                .Invoking(engine => engine.GetDependency(It.IsNotNull<Type>()))
                .Should().Throw<ObjectDisposedException>();

            _engine
                .Invoking(engine => engine.GetRequiredDependency(It.IsNotNull<Type>()))
                .Should().Throw<ObjectDisposedException>();
        }

        [Fact]
        public void ThrowIfRequiredDependencyNotFound()
        {
            _engine
                .Invoking(dependencies => dependencies.GetRequiredDependency(typeof(Boo)))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}