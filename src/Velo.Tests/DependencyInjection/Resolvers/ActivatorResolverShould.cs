using System;
using System.Reflection;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Velo.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class ActivatorResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Mock<IDependencyScope> _scope;
        private readonly Type _implementation;
        private readonly ActivatorResolver _resolver;

        public ActivatorResolverShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(IBooRepository);
            _implementation = typeof(BooRepository);
            _scope = MockScope();

            _resolver = new ActivatorResolver(_implementation);
        }

        [Fact]
        public void CallScope()
        {
            var constructor = ReflectionUtils.GetConstructor(_implementation);

            _resolver.Resolve(_contract, _scope.Object);

            _scope.Verify(scope => scope.Activate(_implementation, constructor));
        }
        
        [Fact]
        public void ResolveInstance()
        {
            var instance = new Mock<IBooRepository>();

            _scope
                .Setup(scope => scope.Activate(_implementation, It.IsNotNull<ConstructorInfo>()))
                .Returns(instance.Object);

            _resolver.Resolve(_contract, _scope.Object).Should().Be(instance.Object);
        }
    }
}