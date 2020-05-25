using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Mapping;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Xunit;

namespace Velo.Tests.DependencyInjection
{
    public class DependencyCollectionShould : DITestClass
    {
        private readonly Type _contract;
        private readonly DependencyCollection _dependencies;
        private readonly Type _implementation;

        public DependencyCollectionShould()
        {
            _contract = typeof(IBooRepository);
            _dependencies = new DependencyCollection();
            _implementation = typeof(BooRepository);
        }

        [Fact]
        public void Add()
        {
            var dependency = MockDependency(contract: _contract);

            _dependencies.Add(dependency.Object);

            _dependencies.Contains(_contract).Should().BeTrue();
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddDependency(DependencyLifetime lifetime)
        {
            _dependencies.AddDependency(_contract, _implementation, lifetime);

            var dependency = _dependencies.GetRequiredDependency(_contract);

            dependency.Contracts.Should().Contain(_contract);
            dependency.Implementation.Should().Be(_implementation);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddGenericDependency(DependencyLifetime lifetime)
        {
            var genericContract = typeof(IMapper<>);
            var genericImplementation = typeof(CompiledMapper<>);

            _dependencies.AddDependency(genericContract, genericImplementation, lifetime);

            var element = typeof(Boo);
            var contract = genericContract.MakeGenericType(element);
            var implementation = genericImplementation.MakeGenericType(element);

            var dependency = _dependencies.GetRequiredDependency(contract);

            dependency.Contracts.Should().Contain(contract);
            dependency.Implementation.Should().Be(implementation);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddDependencyWithManyContracts(DependencyLifetime lifetime)
        {
            var additionalContract = typeof(IRepository<Boo>);
            var contracts = new[] {_contract, additionalContract};

            _dependencies.AddDependency(contracts, _implementation, lifetime);

            _dependencies.Contains(additionalContract).Should().BeTrue();
            _dependencies.Contains(_contract).Should().BeTrue();
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddDependencyWithBuilder(DependencyLifetime lifetime)
        {
            var contracts = new[] {_contract};
            var builder = Mock.Of<Func<IServiceProvider, BooRepository>>();

            _dependencies.AddDependency(contracts, builder, lifetime);

            _dependencies.Contains(_contract).Should().BeTrue();
        }

        [Fact]
        public void AddFactory()
        {
            var factory = MockDependencyFactory(_contract);

            _dependencies.AddFactory(factory.Object);

            _dependencies.Contains(_contract).Should().BeTrue();
        }

        [Fact]
        public void AddInstance()
        {
            var contracts = new[] {_contract};
            var instance = Mock.Of<IBooRepository>();

            var actual = _dependencies
                .AddInstance(contracts, instance)
                .BuildProvider()
                .GetRequired(_contract);

            actual.Should().Be(instance);
        }

        [Fact]
        public void AddInstanceWithManyContracts()
        {
            var contracts = new[] {_contract, _implementation};
            var instance = Mock.Of<IBooRepository>();

            var provider = _dependencies
                .AddInstance(contracts, instance)
                .BuildProvider();

            foreach (var contract in contracts)
            {
                var actual = provider.GetRequired(contract);
                actual.Should().Be(instance);
            }
        }

        [Fact]
        public void AddProviderToDependencyCollection()
        {
            var provider = _dependencies.BuildProvider();

            provider.GetRequired<IServiceProvider>().Should().Be(provider);
            provider.GetRequired<DependencyProvider>().Should().Be(provider);
        }

        [Fact]
        public void BuildProvider()
        {
            _dependencies
                .Invoking(dependencies => dependencies.BuildProvider())
                .Should().NotThrow()
                .Which.Should().BeAssignableTo<IServiceProvider>();
        }

        [Fact]
        public void GetApplicable()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _dependencies.Add(dependency);

            _dependencies.GetApplicable(_contract).Should().Contain(dependency);
        }

        [Fact]
        public void GetDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _dependencies.Add(dependency);

            _dependencies.GetDependency(_contract).Should().Be(dependency);
            _dependencies.GetDependency(typeof(Boo)).Should().BeNull();
        }

        [Fact]
        public void GetRequiredDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _dependencies.Add(dependency);

            _dependencies.GetRequiredDependency(_contract).Should().Be(dependency);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void GetDependencyLifetime(DependencyLifetime lifetime)
        {
            var dependency = MockDependency(lifetime, _contract).Object;

            _dependencies.Add(dependency);

            _dependencies.GetLifetime(_contract).Should().Be(lifetime);
        }

        [Fact]
        public void NotContains()
        {
            _dependencies.Contains(typeof(Boo)).Should().BeFalse();
        }

        [Fact]
        public void NotThrowIfApplicableNotFound()
        {
            _dependencies
                .Invoking(dependencies => dependencies.GetApplicable(typeof(Boo)))
                .Should().NotThrow()
                .Which.Should().BeEmpty();
        }

        [Fact]
        public void NotThrowIfRemovedContractNotExists()
        {
            _dependencies
                .Invoking(dependencies => dependencies.Remove(typeof(Boo)))
                .Should().NotThrow()
                .Which.Should().BeFalse();
        }
        
        [Fact]
        public void RemoveExistsDependency()
        {
            var dependency = MockDependency(contract: _contract).Object;

            _dependencies.Add(dependency);
            
            _dependencies.Remove(_contract).Should().BeTrue();
        }

        [Fact]
        public void ScanAssemblies()
        {
            _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<IBooRepository>()
                    .RegisterAsSingleton<IBooRepository>());

            _dependencies.Contains(typeof(IBooRepository));
        }
        
        [Fact]
        public void ThrowIfAddNonGenericImplementationWithGenericContract()
        {
            _dependencies
                .Invoking(collection => collection
                    .AddDependency(typeof(IMapper<>), typeof(CompiledMapper<Boo>), DependencyLifetime.Singleton))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfAddNonApplicableBuilder()
        {
            var contracts = new[] {_contract};
            var builder = Mock.Of<Func<IServiceProvider, Boo>>();

            _dependencies
                .Invoking(collection => collection.AddDependency(contracts, builder, DependencyLifetime.Singleton))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfRequiredDependencyNotFound()
        {
            _dependencies
                .Invoking(dependencies => dependencies.GetRequiredDependency(typeof(Boo)))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}