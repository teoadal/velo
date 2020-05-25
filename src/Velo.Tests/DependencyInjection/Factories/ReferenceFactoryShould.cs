using System;
using System.IO;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Factories
{
    public class ReferenceFactoryShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Type _dependencyType;
        private readonly Mock<IDependencyEngine> _engine;

        private readonly ReferenceFactory _factory;

        public ReferenceFactoryShould()
        {
            _dependencyType = typeof(IBooRepository);
            _engine = new Mock<IDependencyEngine>();
            _contract = typeof(IReference<>).MakeGenericType(_dependencyType);

            _factory = new ReferenceFactory();
        }

        [Theory]
        [InlineData(typeof(Boo))]
        [InlineData(typeof(Boo[]))]
        [InlineData(typeof(IBooRepository))]
        public void Applicable(Type type)
        {
            var contract = typeof(IReference<>).MakeGenericType(type);
            _factory.Applicable(contract).Should().BeTrue();
        }

        [Fact]
        public void BuildDependency()
        {
            var dependency = Act();

            dependency.Should().NotBeNull();
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void BuildValidImplementation(DependencyLifetime lifetime)
        {
            SetupRequiredDependency(_engine, _dependencyType, lifetime);

            var engine = _engine.Object;

            switch (lifetime)
            {
                case DependencyLifetime.Scoped:
                    _factory
                        .Invoking(factory => factory.BuildDependency(_contract, engine))
                        .Should().Throw<InvalidDataException>();
                    break;
                case DependencyLifetime.Singleton:
                    var singletonDependency = _factory.BuildDependency(_contract, engine);
                    singletonDependency.Implementation.GenericTypeArguments[0].Should().Be(_dependencyType);
                    break;
                case DependencyLifetime.Transient:
                    var transientDependency = _factory.BuildDependency(_contract, engine);
                    transientDependency.Implementation.GenericTypeArguments[0].Should().Be(_dependencyType);
                    break;
            }
        }

        [Fact]
        public void CallDependencyEngine()
        {
            Act();

            _engine.Verify(engine => engine.GetRequiredDependency(_dependencyType));
        }

        #region Reference

        [Fact]
        public void Reference_SingletonApplicable()
        {
            var dependency = Act();

            dependency.Applicable(_contract).Should().BeTrue();
        }

        [Fact]
        public void Reference_SingletonCallServiceProvider()
        {
            var services = new Mock<IServiceProvider>();
            var reference = ResolveReference(services, out _);

            reference
                .Invoking(r => r.Value)
                .Should().NotThrow();

            services.Verify(s => s.GetService(_dependencyType));
        }

        [Fact]
        public void Reference_SingletonSameValue()
        {
            var services = new Mock<IServiceProvider>();
            var reference = ResolveReference(services, out _);

            var first = reference.Value;
            first.Should().Be(reference.Value);
        }

        [Fact]
        public void Reference_SingletonDispose()
        {
            var services = new Mock<IServiceProvider>();
            ResolveReference(services, out var dependency);

            dependency
                .Invoking(d => d.Dispose())
                .Should().NotThrow();
        }

        [Fact]
        public void Reference_TransientApplicable()
        {
            var dependency = Act(DependencyLifetime.Transient);
            dependency.Applicable(_contract).Should().BeTrue();
        }

        [Fact]
        public void Reference_TransientCallServiceProvider()
        {
            var services = new Mock<IServiceProvider>();
            var reference = ResolveReference(services, out _);

            reference
                .Invoking(r => r.Value)
                .Should().NotThrow();

            services.Verify(s => s.GetService(_dependencyType));
        }
        
        [Fact]
        public void Reference_TransientNotSameValue()
        {
            var services = new Mock<IServiceProvider>();
            var reference = ResolveReference(services, out _, DependencyLifetime.Transient);

            var first = reference.Value;
            first.Should().NotBe(reference.Value);
        }
        
        [Fact]
        public void Reference_TransientDispose()
        {
            var services = new Mock<IServiceProvider>();
            ResolveReference(services, out var dependency, DependencyLifetime.Transient);

            dependency
                .Invoking(d => d.Dispose())
                .Should().NotThrow();
        }
        
        #endregion

        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(IBooRepository)).Should().BeFalse();
        }

        private IDependency Act(DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            SetupRequiredDependency(_engine, _dependencyType, lifetime);

            return _factory.BuildDependency(_contract, _engine.Object);
        }

        private IReference<IBooRepository> ResolveReference(
            Mock<IServiceProvider> services, 
            out IDependency dependency,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            dependency = Act(lifetime);

            services
                .Setup(s => s.GetService(typeof(IServiceProvider)))
                .Returns(() => services.Object);

            switch (lifetime)
            {
                case DependencyLifetime.Singleton:
                    services
                        .Setup(s => s.GetService(_dependencyType))
                        .Returns(MockOf(_dependencyType));
                    break;
                case DependencyLifetime.Transient:
                    services
                        .Setup(s => s.GetService(_dependencyType))
                        .Returns(() => MockOf(_dependencyType));
                    break;
            }

            return (IReference<IBooRepository>) dependency.GetInstance(_contract, services.Object);
        }
    }
}