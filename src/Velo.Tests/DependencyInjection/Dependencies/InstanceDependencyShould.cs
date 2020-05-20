using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public sealed class InstanceDependencyShould : DITestClass
    {
        private readonly Type[] _contracts;
        private readonly InstanceDependency _dependency;
        private readonly Mock<IBooRepository> _instance;

        public InstanceDependencyShould()
        {
            _instance = new Mock<IDisposable>().As<IBooRepository>();

            _contracts = new[] {typeof(IBooRepository), typeof(BooRepository)};
            _dependency = new InstanceDependency(_contracts, _instance.Object);
        }

        [Fact]
        public void Applicable()
        {
            foreach (var contract in _contracts)
            {
                _dependency.Applicable(contract).Should().BeTrue();
            }
        }

        [Fact]
        public void DisposeInstance()
        {
            _dependency.Dispose();
            _instance.As<IDisposable>().Verify(i => i.Dispose());
        }

        [Fact]
        public void HasValidContracts()
        {
            _dependency.Contracts.Should().Contain(_contracts);
        }

        [Fact]
        public void GetInstance()
        {
            _dependency
                .GetInstance(It.IsAny<Type>(), It.IsAny<IServiceProvider>())
                .Should().Be(_instance.Object);
        }

        [Fact]
        public void ImplementResolver()
        {
            _dependency.Should().BeAssignableTo<DependencyResolver>();
        }

        [Fact]
        public void NotApplicable()
        {
            var notApplicable = typeof(IDependency);
            _dependency.Applicable(notApplicable).Should().BeFalse();
        }

        [Fact]
        public void ResolveInstance()
        {
            var resolver = (DependencyResolver) _dependency;

            resolver
                .Resolve(It.IsAny<Type>(), It.IsNotNull<IServiceProvider>())
                .Should().Be(_instance.Object);
        }

        [Fact]
        public void SameResolver()
        {
            _dependency.Implementation.Should().Be(_instance.Object.GetType());
        }
        
        [Fact]
        public void Singleton()
        {
            _dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
        }
    }
}