using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class CompiledResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Mock<IDependencyScope> _scope;
        private readonly Type _implementation;
        private readonly CompiledResolver _resolver;

        private readonly Dictionary<Type, Mock<IDependency>> _dependencies;
        private DependencyLifetime _dependenciesLifetime;

        public CompiledResolverShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(IBooRepository);
            _dependencies = new Dictionary<Type, Mock<IDependency>>();
            _implementation = typeof(BooRepository);
            _scope = MockScope();

            var engine = new Mock<IDependencyEngine>();
            engine
                .Setup(e => e.GetRequiredDependency(_contract))
                .Returns<Type>(DependencyBuilder);

            _resolver = new CompiledResolver(_implementation, engine.Object);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void CallCollectedDependencies(DependencyLifetime lifetime)
        {
            _dependenciesLifetime = lifetime;

            _resolver.Resolve(_contract, _scope.Object);

            foreach (var (type, dependency) in _dependencies)
            {
                dependency.Verify(d => d.GetInstance(type, _scope.Object));
            }
        }

        [Fact]
        public void ResolveInstance()
        {
            var instance = _resolver.Resolve(_contract, _scope.Object);
            instance.Should().BeOfType(_implementation);
        }

        private IDependency DependencyBuilder(Type contract)
        {
            var dependency = new Mock<IDependency>();
            dependency
                .SetupGet(d => d.Lifetime)
                .Returns(_dependenciesLifetime);

            var mockObjectType = typeof(Mock<>).MakeGenericType(contract);
            var mock = (Mock) Activator.CreateInstance(mockObjectType);

            dependency
                .Setup(d => d.GetInstance(contract, _scope.Object))
                .Returns(mock!.Object);

            _dependencies.Add(contract, dependency);

            return dependency.Object;
        }
    }
}