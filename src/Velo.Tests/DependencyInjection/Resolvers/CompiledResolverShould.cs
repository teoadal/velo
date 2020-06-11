using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.Settings.Provider;
using Velo.TestsModels.Boos;
using Velo.TestsModels.DependencyInjection;
using Xunit;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class CompiledResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Mock<IDependencyEngine> _engine;
        private readonly Type _implementation;
        private readonly IServiceProvider _services;

        private readonly CompiledResolver _resolver;

        public CompiledResolverShould()
        {
            _contract = typeof(IBooRepository);
            _implementation = typeof(BooRepository);

            _engine = new Mock<IDependencyEngine>();
            _services = Mock.Of<IServiceProvider>();

            _resolver = new CompiledResolver(_implementation);
            _resolver.Init(DependencyLifetime.Scoped, _engine.Object);
        }

        [Theory, AutoData]
        public void CreateBuilderOnce(int count)
        {
            Act();

            _engine.Verify(engine => engine.GetDependency(It.IsNotNull<Type>()));
            _engine.Verify(engine => engine.GetRequiredDependency(It.IsNotNull<Type>()));

            for (var i = 0; i < EnsureValid(count); i++)
            {
                Act();
            }

            _engine.VerifyNoOtherCalls();
        }

        [Fact]
        public void CallDependencyEngine()
        {
            Act();

            _engine.Verify(engine => engine.GetRequiredDependency(It.IsNotNull<Type>()));
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void CallDependencyInstanceOnBuildOrOnResolve(DependencyLifetime lifetime)
        {
            var contract = typeof(ISettingsProvider);
            var dependency = MockDependency<ISettingsProvider>(lifetime, _services);

            _engine
                .Setup(engine => engine.GetRequiredDependency(contract))
                .Returns(dependency.Object);

            Act();

            dependency.Verify(d => d.GetInstance(contract, _services));
        }
        
        [Fact]
        public void ResolveInstance()
        {
            var instance = Act();
            instance.Should().BeOfType(_implementation);
        }

        [Fact]
        public void ThrowIfPublicConstructorNotFound()
        {
            var implementation = typeof(PrivateConstructorClass);
            Assert.Throws<KeyNotFoundException>(() => new CompiledResolver(implementation));
        }

        private object Act()
        {
            return _resolver
                .Invoking(resolver => resolver.Resolve(_contract, _services))
                .Should().NotThrow().Which;
        }
    }
}