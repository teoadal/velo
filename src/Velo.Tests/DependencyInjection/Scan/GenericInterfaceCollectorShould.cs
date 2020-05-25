using System;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Xunit;

namespace Velo.Tests.DependencyInjection.Scan
{
    public class GenericInterfaceCollectorShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Type _contractImplementation;
        private readonly DependencyCollection _collection;
        private readonly Type _implementation;

        public GenericInterfaceCollectorShould()
        {
            _collection = new DependencyCollection();
            _contract = typeof(IRepository<>);
            _contractImplementation = _contract.MakeGenericType(typeof(Boo));
            _implementation = typeof(BooRepository);
        }
        [Fact]
        public void Register()
        {
            var collector = new GenericInterfaceCollector(_contract, DependencyLifetime.Singleton);

            collector.TryRegister(_collection, _implementation);

            var dependency = _collection.GetRequiredDependency(_contractImplementation);

            dependency.Contracts.Should().Contain(_contractImplementation);
            dependency.Implementation.Should().Be(_implementation);
            dependency.Applicable(_contractImplementation).Should().BeTrue();
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void RegisterWithValidLifetime(DependencyLifetime lifetime)
        {
            var collector = new GenericInterfaceCollector(_contract, lifetime);

            collector.TryRegister(_collection, _implementation);

            var dependency = _collection.GetRequiredDependency(_contractImplementation);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void NotRegister()
        {
            var collector = new GenericInterfaceCollector(_contract, DependencyLifetime.Singleton);

            collector.TryRegister(_collection, typeof(Boo));

            _collection.Contains(_contract).Should().BeFalse();
        }
    }
}