using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Velo.TestsModels.DependencyInjection;
using Velo.Utils;
using Xunit;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class ActivatorResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Type _implementation;
        private readonly Mock<IServiceProvider> _services;

        private readonly ActivatorResolver _resolver;

        public ActivatorResolverShould()
        {
            _contract = typeof(IBooRepository);
            _implementation = typeof(BooRepository);

            _services = new Mock<IServiceProvider>();

            foreach (var parameter in ReflectionUtils.GetConstructorParameters(_implementation))
            {
                var parameterType = parameter.ParameterType;
                _services
                    .Setup(services => services.GetService(parameterType))
                    .Returns(MockOf(parameterType));
            }

            _resolver = new ActivatorResolver(_implementation);
        }

        [Theory, AutoData]
        public void ActivateManyTime(int count)
        {
            count = EnsureValid(count);

            for (var i = 0; i < count; i++)
            {
                _resolver
                    .Invoking(resolver => resolver.Resolve(_contract, _services.Object))
                    .Should().NotThrow();
            }

            foreach (var parameter in ReflectionUtils.GetConstructorParameters(_implementation))
            {
                _services
                    .Verify(services => services
                        .GetService(parameter.ParameterType), Times.Exactly(count));
            }
        }

        [Fact]
        public void CallServiceProvider()
        {
            _resolver
                .Invoking(resolver => resolver.Resolve(_contract, _services.Object))
                .Should().NotThrow();

            _services.Verify(services => services.GetService(It.IsNotNull<Type>()));
        }

        [Fact]
        public void ResolveInstance()
        {
            var instance = _resolver.Resolve(_contract, _services.Object);
            instance.Should().BeOfType(_implementation);
        }

        [Fact]
        public void ThrowIfDependencyNotRegistered()
        {
            _services.Reset();

            _resolver
                .Invoking(resolver => resolver.Resolve(_contract, _services.Object))
                .Should().Throw<KeyNotFoundException>();
        }
        
        [Fact]
        public void ThrowIfPublicConstructorNotFound()
        {
            var implementation = typeof(PrivateConstructorClass);
            Assert.Throws<KeyNotFoundException>(() => new ActivatorResolver(implementation));
        }
    }
}