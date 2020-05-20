using System;
using System.IO;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class DependencyResolverShould : DITestClass
    {
        private readonly Type _implementation;
        private readonly IBooRepository _instance;
        private readonly IServiceProvider _services;

        private readonly DependencyResolver _resolver;

        public DependencyResolverShould()
        {
            _implementation = typeof(BooRepository);
            _instance = Mock.Of<IBooRepository>();
            _resolver = MockResolver(_implementation, (contract, scope) => _instance).Object;

            _services = Mock.Of<IServiceProvider>();
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
            _resolver.Implementation.Should().Be(_implementation);
        }

        [Fact]
        public void ResolveInstance()
        {
            _resolver
                .Resolve(_implementation, _services)
                .Should().Be(_instance);
        }

        [Fact]
        public void ThrowCircularDependency()
        {
            Mock<DependencyResolver> resolver = null; // circular imitation
            resolver = MockResolver(_implementation, (contract, scope) =>
            {
                // ReSharper disable once AccessToModifiedClosure
                resolver!.Object.Resolve(contract, scope);
                return null;
            });

            Assert.Throws<TypeAccessException>(() => resolver.Object.Resolve(_implementation, _services));
        }

        [Fact]
        public void ThrowIfBuildWithInvalidLifetime()
        {
            const DependencyLifetime badLifetime = (DependencyLifetime) byte.MaxValue;

            var engine = Mock.Of<IDependencyEngine>();
            Assert
                .Throws<InvalidDataException>(() => DependencyResolver
                    .Build(badLifetime, _implementation, engine));
        }
    }
}