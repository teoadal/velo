using System;
using System.IO;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting;
using Xunit;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public class DependencyShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Mock<Dependency> _dependency;
        private readonly Type _implementation;

        public DependencyShould()
        {
            _contract = typeof(IBooRepository);
            _implementation = typeof(BooRepository);

            _dependency = BuildDependency(new[] {_contract, _implementation});
        }

        [Fact]
        public void Applicable()
        {
            _dependency.Object.Applicable(_implementation).Should().BeTrue();
        }

        [Fact]
        public void ApplicableByContravariantInterface()
        {
            var contracts = new[] {typeof(ICommandBehaviour<IMeasureCommand>)};
            var dependency = BuildDependency(contracts);

            var contravariantInterface = typeof(ICommandBehaviour<TestsModels.Emitting.Boos.Create.Command>);

            dependency.Object.Applicable(contravariantInterface).Should().BeTrue();
        }

        [Fact]
        public void ApplicableByInterface()
        {
            var dependency = _dependency.Object;

            dependency.Applicable(_contract).Should().BeTrue();
            foreach (var implementedInterface in _contract.GetInterfaces())
            {
                dependency.Applicable(implementedInterface).Should().BeTrue();
            }
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void Build(DependencyLifetime lifetime)
        {
            var resolver = MockResolver(_implementation, (_, scope) => null).Object;

            var dependency = Dependency.Build(lifetime, new[] {_contract}, resolver);

            switch (lifetime)
            {
                case DependencyLifetime.Scoped:
                    dependency.Should().BeOfType<ScopedDependency>();
                    break;
                case DependencyLifetime.Singleton:
                    dependency.Should().BeOfType<SingletonDependency>();
                    break;
                case DependencyLifetime.Transient:
                    dependency.Should().BeOfType<TransientDependency>();
                    break;
            }
        }

        [Fact]
        public void HasImplementation()
        {
            var resolver = new Mock<DependencyResolver>(_contract).Object;
            var dependency = new Mock<Dependency>(It.IsNotNull<Type[]>(), resolver, DependencyLifetime.Singleton);

            dependency.Object.Implementation.Should().Be(_contract);
        }

        [Fact]
        public void HasValidContracts()
        {
            _dependency.Object.Contracts.Should().Contain(_contract);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void HasValidLifetime(DependencyLifetime lifetime)
        {
            var dependency = BuildDependency(It.IsNotNull<Type[]>(), lifetime);
            dependency.Object.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void NotApplicable()
        {
            var notApplicable = typeof(DependencyShould);
            _dependency.Object.Applicable(notApplicable).Should().BeFalse();
        }

        [Fact]
        public void NotApplicableByContravariantInterface()
        {
            var contracts = new[] {typeof(ICommandBehaviour<IMeasureCommand>)};
            var dependency = BuildDependency(contracts);

            var notContravariantInterface = typeof(ICommandBehaviour<ICommand>);

            dependency.Object.Applicable(notContravariantInterface).Should().BeFalse();
        }

        [Fact]
        public void NotApplicableByInterface()
        {
            var notApplicable = typeof(IDependency);
            _dependency.Object.Applicable(notApplicable).Should().BeFalse();
        }

        [Fact]
        public void ThrowIfBuildWithInvalidLifetime()
        {
            const DependencyLifetime badLifetime = (DependencyLifetime) byte.MaxValue;

            var resolver = new Mock<DependencyResolver>(_implementation).Object;
            Assert
                .Throws<InvalidDataException>(() => Dependency
                    .Build(badLifetime, new[] {_contract}, resolver));
        }

        private static Mock<Dependency> BuildDependency(Type[] contracts,
            DependencyLifetime lifetime = DependencyLifetime.Scoped)
        {
            return new Mock<Dependency>(contracts, It.IsAny<DependencyResolver>(), lifetime);
        }
    }
}