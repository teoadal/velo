using System;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Scan
{
    public class AssignableCollectorShould : DITestClass
    {
        private readonly Type _contract;
        private readonly DependencyCollection _collection;
        private readonly Type _implementation;

        public AssignableCollectorShould()
        {
            _collection = new DependencyCollection();
            _contract = typeof(IBooRepository);
            _implementation = typeof(BooRepository);
        }

        [Fact]
        public void Register()
        {
            var collector = new AssignableCollector(_contract, DependencyLifetime.Singleton);

            collector.TryRegister(_collection, _implementation);

            var dependency = _collection.GetRequiredDependency(_contract);

            dependency.Contracts.Should().Contain(_contract);
            dependency.Implementation.Should().Be(_implementation);
            dependency.Applicable(_contract).Should().BeTrue();
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void RegisterWithValidLifetime(DependencyLifetime lifetime)
        {
            var collector = new AssignableCollector(_contract, lifetime);

            collector.TryRegister(_collection, _implementation);

            var dependency = _collection.GetRequiredDependency(_contract);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void NotRegister()
        {
            var collector = new AssignableCollector(_contract, DependencyLifetime.Scoped);

            collector.TryRegister(_collection, typeof(Boo));

            _collection.Contains(_contract).Should().BeFalse();
        }
    }
}