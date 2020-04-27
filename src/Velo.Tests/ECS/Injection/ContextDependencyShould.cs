using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Injection;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Injection
{
    public class ContextDependencyShould : ECSTestClass
    {
        private readonly Mock<IActorContext> _actorContext;
        private readonly Type _contract;
        private readonly Mock<IDependencyScope> _scope;

        private readonly ContextDependency<IActorContext> _actorContextDependency;

        public ContextDependencyShould(ITestOutputHelper output) : base(output)
        {
            _actorContext = new Mock<IActorContext>();
            _contract = typeof(IActorFilter<TestComponent1>);
            _scope = new Mock<IDependencyScope>();
            _scope
                .Setup(scope => scope.GetRequiredService(typeof(IActorContext)))
                .Returns(_actorContext.Object);

            _actorContextDependency = new ContextDependency<IActorContext>(
                _contract,
                context => context.GetFilter<TestComponent1>());
        }

        [Fact]
        public void Applicable()
        {
            _actorContextDependency.Applicable(_contract).Should().BeTrue();
        }

        [Fact]
        public void GetInstance()
        {
            _actorContextDependency.GetInstance(_contract, _scope.Object);

            _actorContext.Verify(context => context.GetFilter<TestComponent1>());
        }

        [Fact]
        public void HasContracts()
        {
            _actorContextDependency.Contracts
                .Should().ContainSingle(contract => contract == _contract)
                .And.HaveCount(1);
        }

        [Fact]
        public void HasResolverImplementation()
        {
            _actorContextDependency.Resolver.Implementation.Should().Be(_contract);
        }

        [Fact]
        public void Singleton()
        {
            _actorContextDependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
        }
    }
}