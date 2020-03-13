using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class DependencyResolverShould : DITestClass
    {
        private readonly Type _implementation;
        private readonly Mock<IBooRepository> _instance;
        private readonly Mock<DependencyResolver> _resolver;
        private readonly IDependencyScope _scope;

        public DependencyResolverShould(ITestOutputHelper output) : base(output)
        {
            _implementation = typeof(BooRepository);
            _instance = new Mock<IBooRepository>();
            _resolver = MockResolver(
                _implementation,
                (contract, scope) => _instance.Object);

            _scope = MockScope().Object;
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void Build(DependencyLifetime lifetime)
        {
            var engine = Mock.Of<IDependencyEngine>();
            var resolver = DependencyResolver.Build(lifetime, typeof(BooRepository), engine);

            switch (lifetime)
            {
                case DependencyLifetime.Transient:
                case DependencyLifetime.Scoped:
                    resolver.Should().BeOfType<CompiledResolver>();
                    break;
                case DependencyLifetime.Singleton:
                    resolver.Should().BeOfType<ActivatorResolver>();
                    break;
            }
        }

        [Fact]
        public void HasValidImplementation()
        {
            _resolver.Object.Implementation.Should().Be(_implementation);
        }

        [Fact]
        public void ResolveInstance()
        {
            _resolver.Object
                .Resolve(_implementation, _scope)
                .Should().Be(_instance.Object);
        }

        [Fact]
        public void ThrowCircularDependency()
        {
            Mock<DependencyResolver> resolver = null;
            resolver = MockResolver(_implementation, (contract, scope) =>
            {
                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once AccessToModifiedClosure
                resolver.Object.Resolve(contract, scope);
                return null;
            });

            Assert.Throws<TypeAccessException>(() => resolver.Object.Resolve(_implementation, _scope));
        }
    }
}