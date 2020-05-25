using System;
using System.Reflection;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Xunit;

namespace Velo.Tests.DependencyInjection.Scan
{
    public class DependencyScannerShould : DITestClass
    {
        private readonly DependencyCollection _collection;
        private readonly Type _contract;
        private readonly DependencyScanner _scanner;

        public DependencyScannerShould()
        {
            _collection = new DependencyCollection();
            _contract = typeof(IBooRepository);
            _scanner = new DependencyScanner(_collection).AssemblyOf<IBooRepository>();
        }

        [Fact]
        public void AddAssembly()
        {
            new DependencyScanner(_collection)
                .Assembly(Assembly.GetAssembly(_contract)!)
                .Register(_contract, It.IsAny<DependencyLifetime>())
                .Execute();

            _collection.Contains(_contract);
        }

        [Fact]
        public void AddAssemblyOf()
        {
            new DependencyScanner(_collection)
                .AssemblyOf<IBooRepository>()
                .Register(_contract, It.IsAny<DependencyLifetime>())
                .Execute();

            _collection.Contains(_contract);
        }

        [Fact]
        public void RegisterDependency()
        {
            _scanner
                .Register(_contract, DependencyLifetime.Singleton)
                .Execute();

            var dependency = _collection.GetRequiredDependency(_contract);
            dependency.Contracts.Should().Contain(_contract);
        }
        
        [Fact]
        public void RegisterGenericDependency()
        {
            var genericContract = typeof(IRepository<>);
            
            _scanner
                .Register(genericContract, DependencyLifetime.Scoped)
                .Execute();

            _collection.Contains(genericContract.MakeGenericType(typeof(Boo)));
        }
        
        [Theory, MemberData(nameof(Lifetimes))]
        public void RegisterDependencyWithValidLifetime(DependencyLifetime lifetime)
        {
            _scanner
                .Register(_contract, lifetime)
                .Execute();

            var dependency = _collection.GetRequiredDependency(_contract);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void UseCollector()
        {
            var collector = new Mock<IDependencyCollector>();

            _scanner
                .UseCollector(collector.Object)
                .Execute();

            collector
                .Verify(c => c
                    .TryRegister(_collection, It.IsNotNull<Type>()));
        }
    }
}